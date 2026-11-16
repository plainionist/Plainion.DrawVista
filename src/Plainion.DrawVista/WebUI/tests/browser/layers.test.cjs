const assert = require('node:assert/strict');
const { spawn } = require('node:child_process');
const { once } = require('node:events');
const { mkdtemp, mkdir, readFile, readdir, rm } = require('node:fs/promises');
const { tmpdir } = require('node:os');
const path = require('node:path');
const { DatabaseSync } = require('node:sqlite');
const { test } = require('node:test');
const { chromium } = require('playwright');

const project = path.resolve(__dirname, '..', '..', '..');
const fixture = path.resolve(
  project,
  '..',
  'Plainion.DrawVista.Tests',
  'Fixtures',
  'Layers.drawio',
);
const program = path.join(
  project,
  'bin',
  'Release',
  'net10.0',
  'Plainion.DrawVista.dll',
);
const legacy =
  '<svg xmlns="http://www.w3.org/2000/svg" width="100%" height="100" viewBox="0 0 100 100"><rect x="10" y="10" width="80" height="80" fill="orange"/></svg>';

function contrastRatio(foreground, background) {
  const rgba = (color) => color.match(/[\d.]+/g).map(Number);
  const front = rgba(foreground);
  const back = rgba(background);
  const alpha = front[3] ?? 1;
  const luminance = (channels) => {
    const [red, green, blue] = channels.slice(0, 3).map((value) => {
      const channel = value / 255;
      return channel <= 0.04045
        ? channel / 12.92
        : ((channel + 0.055) / 1.055) ** 2.4;
    });
    return 0.2126 * red + 0.7152 * green + 0.0722 * blue;
  };
  const text = luminance(
    front
      .slice(0, 3)
      .map((value, index) => value * alpha + back[index] * (1 - alpha)),
  );
  const surface = luminance(back);
  return (Math.max(text, surface) + 0.05) / (Math.min(text, surface) + 0.05);
}

async function settle(page) {
  await page.evaluate(
    () =>
      new Promise((resolve) =>
        requestAnimationFrame(() => requestAnimationFrame(resolve)),
      ),
  );
}

async function choices(page) {
  return page
    .locator('.svg-container svg')
    .evaluate((svg) =>
      Array.from(
        svg.querySelectorAll('[data-drawvista-layer-id]'),
        (group) => group.getAttribute('display') !== 'none',
      ),
    );
}

async function toggle(page, name, enabled) {
  await page.getByRole('button', { name: 'Layers', exact: true }).click();
  const checkbox = page.getByRole('checkbox', { name, exact: true });
  if ((await checkbox.getAttribute('aria-checked')) !== String(enabled)) {
    await checkbox.click();
  }
  assert.equal(await checkbox.getAttribute('aria-checked'), String(enabled));
  await page.keyboard.press('Escape');
  await checkbox.waitFor({ state: 'hidden' });
}

async function showPage(page, baseUrl, name) {
  await page.goto(`${baseUrl}/#/?page=${name}`);
  await page.locator('.svg-pan-zoom_viewport').waitFor({ state: 'attached' });
  await page.locator('.svg-container svg').waitFor({ state: 'attached' });
  await settle(page);
}

async function selectPage(page, name, cell) {
  await page.getByRole('combobox').click();
  await page.getByRole('option', { name, exact: true }).click();
  await page.locator(`[data-cell-id="${cell}"]`).waitFor({ state: 'attached' });
  await page.locator('.svg-pan-zoom_viewport').waitFor({ state: 'attached' });
  await settle(page);
}

async function center(locator) {
  const box = await locator.boundingBox();
  assert.ok(box, 'The visible link must have screen bounds');
  return { x: box.x + box.width / 2, y: box.y + box.height / 2 };
}

test(
  'Layers browser acceptance with the real importer, stores, API and SVG',
  { timeout: 180000 },
  async (t) => {
    const root = await mkdtemp(
      path.join(tmpdir(), 'DrawVista.Layers.Browser-'),
    );
    const data = path.join(root, 'Plainion.DrawVista', 'Layers.BrowserTests');
    let server;
    let browser;
    let serverLog = '';
    t.after(async () => {
      if (browser) {
        await browser.close();
      }
      if (server && server.exitCode === null) {
        const exited = once(server, 'exit');
        server.kill();
        await exited;
      }
      await rm(root, { recursive: true, force: true });
    });

    await mkdir(data, { recursive: true });
    const database = new DatabaseSync(path.join(data, 'store.db'));
    database.exec(
      'CREATE TABLE Pages (Name TEXT NOT NULL PRIMARY KEY, Content TEXT, Captions TEXT)',
    );
    database
      .prepare('INSERT INTO Pages VALUES (?, ?, ?)')
      .run('Legacy', legacy, '[]');
    database.close();

    server = spawn(
      'dotnet',
      [
        program,
        '-name',
        'Layers.BrowserTests',
        '-p',
        '0',
        '--HostUri',
        'http://127.0.0.1:0',
      ],
      {
        cwd: path.dirname(program),
        env: {
          ...process.env,
          ALLUSERSPROFILE: root,
          ASPNETCORE_ENVIRONMENT: 'Production',
        },
        stdio: ['ignore', 'pipe', 'pipe'],
      },
    );
    const baseUrl = await new Promise((resolve, reject) => {
      const timeout = setTimeout(
        () => reject(new Error(`Server did not start:\n${serverLog}`)),
        30000,
      );
      server.once('error', (error) => {
        clearTimeout(timeout);
        reject(error);
      });
      server.once('exit', (code) => {
        clearTimeout(timeout);
        reject(new Error(`Server exited ${code}:\n${serverLog}`));
      });
      server.stdout.on('data', (chunk) => {
        serverLog += chunk.toString();
        const match = /Now listening on:\s*(http:\/\/127\.0\.0\.1:\d+)/.exec(
          serverLog,
        );
        if (match) {
          clearTimeout(timeout);
          resolve(match[1]);
        }
      });
      server.stderr.on('data', (chunk) => {
        serverLog += chunk.toString();
      });
    });
    assert.deepEqual(await (await fetch(`${baseUrl}/pageNames`)).json(), [
      'Legacy',
    ]);

    const sourceBytes = await readFile(fixture);
    async function upload(name, bytes) {
      const form = new FormData();
      form.append(name, new Blob([bytes]), name);
      const response = await fetch(`${baseUrl}/upload`, {
        method: 'POST',
        body: form,
      });
      assert.equal(
        response.status,
        200,
        `${await response.text()}\n${serverLog}`,
      );
    }
    await upload('Layers.drawio', sourceBytes);
    const png = await readFile(`${fixture}.png`);
    await upload('Layers.drawio.png', png);
    await upload(
      'Default.drawio',
      Buffer.from(
        '<mxfile><diagram name="Default"><mxGraphModel><root><mxCell id="0"/><mxCell id="1" parent="0"/><mxCell id="box" vertex="1" parent="1"><mxGeometry x="10" y="10" width="80" height="40" as="geometry"/></mxCell></root></mxGraphModel></diagram></mxfile>',
      ),
    );
    const storedSvg = await (
      await fetch(`${baseUrl}/svg?pageName=Layers`)
    ).text();

    browser = await chromium.launch({ channel: 'msedge', headless: true });

    await t.test(
      'toolbar, source defaults, shared framing, stacking and all-off hit testing',
      async () => {
        const context = await browser.newContext({
          viewport: { width: 1200, height: 900 },
        });
        try {
          const page = await context.newPage();
          await showPage(page, baseUrl, 'Layers');
          assert.deepEqual(await choices(page), [true, true, false]);
          const search = await page
            .getByRole('textbox', { name: 'Search', exact: true })
            .boundingBox();
          const button = page.getByRole('button', {
            name: 'Layers',
            exact: true,
          });
          const toolbarBounds = await button.boundingBox();
          assert.ok(toolbarBounds.x >= search.x + search.width);
          assert.equal(await button.isEnabled(), true);
          const buttonStyle = await button.evaluate((element) => {
            const style = getComputedStyle(element);
            return {
              color: style.color,
              underlineColor: style.borderBottomColor,
              underlineWidth: parseFloat(style.borderBottomWidth),
              background: getComputedStyle(element.closest('.q-header'))
                .backgroundColor,
            };
          });
          assert.ok(
            contrastRatio(buttonStyle.color, buttonStyle.background) >= 4.5,
            'The Layers label must be readable against the toolbar',
          );
          assert.ok(
            buttonStyle.underlineWidth > 0 &&
              contrastRatio(
                buttonStyle.underlineColor,
                buttonStyle.background,
              ) >= 3,
            'The Layers button must have a visible underline',
          );
          await button.click();
          await page.getByRole('checkbox').first().waitFor();
          assert.deepEqual(await page.getByRole('checkbox').allTextContents(), [
            'Hidden',
            'Foreground',
            'Background',
          ]);
          const checkboxes = await page
            .getByRole('checkbox')
            .evaluateAll((elements) =>
              elements.map((element) => {
                const box = getComputedStyle(
                  element.querySelector('.q-checkbox__bg'),
                );
                return {
                  checked: element.getAttribute('aria-checked'),
                  background: box.backgroundColor,
                  outline: box.borderColor,
                  checkmark: getComputedStyle(
                    element.querySelector('.q-checkbox__truthy'),
                  ).stroke,
                  menu: getComputedStyle(element.closest('.q-menu'))
                    .backgroundColor,
                };
              }),
            );
          assert.deepEqual(
            checkboxes.map(({ checked }) => checked),
            ['false', 'true', 'true'],
          );
          for (const checkbox of checkboxes) {
            assert.ok(
              contrastRatio(
                checkbox.checked === 'true'
                  ? checkbox.background
                  : checkbox.outline,
                checkbox.menu,
              ) >= 3,
              'Checked and unchecked boxes must be visible against the menu',
            );
            if (checkbox.checked === 'true') {
              assert.ok(
                contrastRatio(checkbox.checkmark, checkbox.background) >= 3,
                'Visible layers must have a visible checkmark',
              );
            }
          }
          await page.keyboard.press('Escape');
          await page.getByRole('checkbox').first().waitFor({ state: 'hidden' });

          await toggle(page, 'Hidden', true);
          const canvas = await page.locator('.svg-container').boundingBox();
          const outlier = await page
            .locator('[data-cell-id="outlier"] rect')
            .boundingBox();
          assert.ok(outlier.x >= canvas.x && outlier.y >= canvas.y);
          assert.ok(outlier.x + outlier.width <= canvas.x + canvas.width);
          assert.ok(outlier.y + outlier.height <= canvas.y + canvas.height);
          const overlap = await page
            .locator('[data-cell-id="foreground-box"] rect')
            .evaluate((rect) => {
              const point = new DOMPoint(
                Number(rect.getAttribute('x')) + 20,
                Number(rect.getAttribute('y')) + 20,
              ).matrixTransform(rect.getScreenCTM());
              return { x: point.x, y: point.y };
            });
          const hitLayer = () =>
            page.evaluate(
              ({ x, y }) =>
                document
                  .elementFromPoint(x, y)
                  ?.closest('[data-drawvista-layer-id]')
                  ?.getAttribute('data-drawvista-layer-id') ?? null,
              overlap,
            );
          assert.equal(await hitLayer(), 'foreground');
          await toggle(page, 'Foreground', false);
          assert.equal(await hitLayer(), '1');
          await toggle(page, 'Background', false);
          assert.equal(await hitLayer(), null);
          await toggle(page, 'Hidden', false);
          assert.deepEqual(await choices(page), [false, false, false]);
          assert.equal(
            await page
              .locator('.svg-container svg')
              .evaluate(
                (svg) =>
                  Array.from(
                    svg.querySelectorAll(
                      'rect,path,image,circle,ellipse,polygon,polyline,text,foreignObject',
                    ),
                  ).filter((element) => element.getClientRects().length > 0)
                    .length,
              ),
            0,
          );
        } finally {
          await context.close();
        }
      },
    );

    await t.test(
      'toggling preserves the actual viewport and non-default zoom/pan transform',
      async () => {
        const context = await browser.newContext({
          viewport: { width: 1200, height: 900 },
        });
        try {
          const page = await context.newPage();
          await showPage(page, baseUrl, 'Layers');
          const viewport = page.locator('.svg-pan-zoom_viewport');
          const initial = await viewport.getAttribute('transform');
          const canvas = await page.locator('.svg-container').boundingBox();
          await page.mouse.move(
            canvas.x + canvas.width / 2,
            canvas.y + canvas.height / 2,
          );
          await page.mouse.wheel(0, -300);
          await page.waitForFunction(
            (before) =>
              document
                .querySelector('.svg-pan-zoom_viewport')
                ?.getAttribute('transform') !== before,
            initial,
          );
          const zoomed = await viewport.getAttribute('transform');
          await page.mouse.move(canvas.x + 30, canvas.y + 30);
          await page.mouse.down();
          await page.mouse.move(canvas.x + 90, canvas.y + 70, { steps: 5 });
          await page.mouse.up();
          await page.waitForFunction(
            (before) =>
              document
                .querySelector('.svg-pan-zoom_viewport')
                ?.getAttribute('transform') !== before,
            zoomed,
          );
          await settle(page);
          const before = await viewport.getAttribute('transform');
          const dimensions = await page
            .locator('.svg-container svg')
            .evaluate((svg) =>
              ['width', 'height', 'viewBox'].map((attribute) =>
                svg.getAttribute(attribute),
              ),
            );
          await page.evaluate(() => {
            window.layersViewport = document.querySelector(
              '.svg-pan-zoom_viewport',
            );
          });
          await toggle(page, 'Hidden', true);
          await toggle(page, 'Background', false);
          await settle(page);
          assert.equal(await viewport.getAttribute('transform'), before);
          assert.deepEqual(
            await page
              .locator('.svg-container svg')
              .evaluate((svg) =>
                ['width', 'height', 'viewBox'].map((attribute) =>
                  svg.getAttribute(attribute),
                ),
              ),
            dimensions,
          );
          assert.equal(
            await page.evaluate(
              () =>
                window.layersViewport ===
                document.querySelector('.svg-pan-zoom_viewport'),
            ),
            true,
          );
        } finally {
          await context.close();
        }
      },
    );

    await t.test(
      'generated/external links activate only when their layer is visible',
      async () => {
        const context = await browser.newContext({
          viewport: { width: 1200, height: 900 },
        });
        await context.route('https://example.com/**', (route) =>
          route.fulfill({ body: 'External link' }),
        );
        try {
          const page = await context.newPage();
          await showPage(page, baseUrl, 'Layers');
          await toggle(page, 'Hidden', true);
          const generated = page.locator(
            '[data-drawvista-layer-id="hidden"] a[href*="page=Second"]',
          );
          const external = page.locator('[data-cell-id="external-link"] a');
          const generatedPoint = await center(generated);
          const externalPoint = await center(external);
          const popupOpened = page.waitForEvent('popup');
          await external.click();
          const popup = await popupOpened;
          await popup.waitForURL('https://example.com/layers');
          await popup.close();
          await generated.click();
          await page.waitForURL(/page=Second/);
          await page
            .locator('[data-cell-id="second-background"]')
            .waitFor({ state: 'attached' });

          await selectPage(page, 'Layers', 'background-box');
          await toggle(page, 'Hidden', false);
          const url = page.url();
          for (const point of [generatedPoint, externalPoint]) {
            assert.equal(
              await page.evaluate(
                ({ x, y }) =>
                  Boolean(document.elementFromPoint(x, y)?.closest('a')),
                point,
              ),
              false,
            );
            await page.mouse.click(point.x, point.y);
          }
          await settle(page);
          assert.equal(page.url(), url);
          assert.equal(context.pages().length, 1);
        } finally {
          await context.close();
        }
      },
    );

    await t.test(
      'independent page choices survive navigation and refresh but not a fresh tab/session',
      async () => {
        const context = await browser.newContext({
          viewport: { width: 1200, height: 900 },
        });
        try {
          const page = await context.newPage();
          await showPage(page, baseUrl, 'Layers');
          await toggle(page, 'Background', false);
          await toggle(page, 'Hidden', true);
          await selectPage(page, 'Second', 'second-background');
          assert.deepEqual(await choices(page), [true, false, true]);
          await toggle(page, 'Foreground', true);
          await toggle(page, 'Hidden', false);
          await selectPage(page, 'Layers', 'background-box');
          assert.deepEqual(await choices(page), [false, true, true]);
          await page.reload();
          await page
            .locator('.svg-pan-zoom_viewport')
            .waitFor({ state: 'attached' });
          assert.deepEqual(await choices(page), [false, true, true]);
          await selectPage(page, 'Second', 'second-background');
          assert.deepEqual(await choices(page), [true, true, false]);
          await page.reload();
          await page
            .locator('.svg-pan-zoom_viewport')
            .waitFor({ state: 'attached' });
          assert.deepEqual(await choices(page), [true, true, false]);
          const fresh = await context.newPage();
          await showPage(fresh, baseUrl, 'Layers');
          assert.deepEqual(await choices(fresh), [true, true, false]);
        } finally {
          await context.close();
        }
        const session = await browser.newContext();
        try {
          const page = await session.newPage();
          await showPage(page, baseUrl, 'Second');
          assert.deepEqual(await choices(page), [true, false, true]);
        } finally {
          await session.close();
        }
      },
    );

    await t.test(
      'legacy/default-only states are disabled and hidden captions navigate without revealing layers',
      async () => {
        const context = await browser.newContext({
          viewport: { width: 1200, height: 900 },
        });
        try {
          const page = await context.newPage();
          await showPage(page, baseUrl, 'Legacy');
          assert.equal(
            await page
              .getByRole('button', { name: 'Layers', exact: true })
              .isDisabled(),
            true,
          );
          assert.equal(
            await page.locator('.svg-container rect').isVisible(),
            true,
          );
          await showPage(page, baseUrl, 'Default');
          assert.equal(
            await page
              .getByRole('button', { name: 'Layers', exact: true })
              .isDisabled(),
            true,
          );
          assert.deepEqual(await choices(page), [true]);
          await selectPage(page, 'Layers', 'background-box');
          await toggle(page, 'Background', false);
          await selectPage(page, 'Second', 'second-background');
          await page
            .getByRole('textbox', { name: 'Search', exact: true })
            .fill('HiddenOnly');
          await page
            .locator('.search-results-container')
            .getByRole('link', { name: 'Layers', exact: true })
            .click();
          await page
            .locator('[data-cell-id="background-box"]')
            .waitFor({ state: 'attached' });
          await page
            .locator('.svg-pan-zoom_viewport')
            .waitFor({ state: 'attached' });
          await settle(page);
          assert.deepEqual(await choices(page), [false, true, false]);
          assert.equal(
            await page.locator('[data-cell-id="outlier"] rect').isVisible(),
            false,
          );
        } finally {
          await context.close();
        }
      },
    );

    assert.equal(
      await (await fetch(`${baseUrl}/svg?pageName=Layers`)).text(),
      storedSvg,
    );
    assert.deepEqual(await readFile(fixture), sourceBytes);
    assert.deepEqual(await readFile(`${fixture}.png`), png);
    assert.deepEqual(await readdir(path.join(data, 'input')), []);
  },
);

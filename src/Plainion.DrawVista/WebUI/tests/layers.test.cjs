const assert = require('node:assert/strict');
const { test } = require('node:test');

function storage() {
  const values = new Map();
  return {
    getItem: (key) => values.get(key) ?? null,
    setItem: (key, value) => values.set(key, value),
  };
}

const source = () => [
  { id: '1', name: 'Background', visible: true },
  { id: 'foreground', name: 'Foreground', visible: true },
  { id: 'hidden', name: 'Hidden', visible: false },
];

test('layer choices restore independently by page and do not mutate source defaults', async () => {
  const { saveLayerPreferences, restoreLayerPreferences } =
    await import('../src/components/svgLayers.ts');
  const session = storage();
  const first = source();
  const choices = first.map((layer) => ({ ...layer, visible: !layer.visible }));
  saveLayerPreferences('Page / one', choices, session);
  saveLayerPreferences(
    'Page two',
    source().map((layer) => ({ ...layer, visible: false })),
    session,
  );

  assert.deepEqual(
    restoreLayerPreferences('Page / one', source(), session),
    choices,
  );
  assert.deepEqual(
    restoreLayerPreferences('Page two', source(), session).map(
      (layer) => layer.visible,
    ),
    [false, false, false],
  );
  assert.deepEqual(
    restoreLayerPreferences('Fresh page', first, session),
    source(),
  );
  assert.deepEqual(
    restoreLayerPreferences('Page / one', source(), storage()),
    source(),
  );
  assert.deepEqual(first, source());
});

test('new layer IDs use source visibility and stale IDs are ignored', async () => {
  const { saveLayerPreferences, restoreLayerPreferences } =
    await import('../src/components/svgLayers.ts');
  const session = storage();
  saveLayerPreferences(
    'Page',
    [{ id: '1', name: 'Background', visible: false }],
    session,
  );
  const current = [...source(), { id: 'new', name: 'New', visible: false }];

  assert.deepEqual(
    restoreLayerPreferences('Page', current, session).map(
      (layer) => layer.visible,
    ),
    [false, true, false, false],
  );
  assert.deepEqual(restoreLayerPreferences('Page', [], session), []);
});

test('invalid or unavailable preference storage surfaces an error', async () => {
  const { saveLayerPreferences, restoreLayerPreferences } =
    await import('../src/components/svgLayers.ts');
  for (const saved of [
    'invalid JSON',
    '{}',
    '[["1", "false"]]',
    '[["1"]]',
    '[null]',
  ]) {
    assert.throws(() =>
      restoreLayerPreferences('Page', source(), { getItem: () => saved }),
    );
  }
  assert.throws(
    () =>
      restoreLayerPreferences('Page', source(), {
        getItem: () => {
          throw new Error('Storage unavailable');
        },
      }),
    /Storage unavailable/,
  );
  assert.throws(
    () =>
      saveLayerPreferences('Page', source(), {
        setItem: () => {
          throw new Error('Storage full');
        },
      }),
    /Storage full/,
  );
});

const assert = require('node:assert/strict');
const { once } = require('node:events');
const { mkdtemp, mkdir, writeFile, rm } = require('node:fs/promises');
const { createRequire } = require('node:module');
const { tmpdir } = require('node:os');
const path = require('node:path');
const { test } = require('node:test');

const quasarRequire = createRequire(require.resolve('@quasar/app-vite/package.json'));
const archiverRequire = createRequire(quasarRequire.resolve('archiver'));
const file = archiverRequire('archiver-utils/file');

test('archiver-utils supports the maintained glob version', async (t) => {
  const directory = await mkdtemp(path.join(tmpdir(), 'drawvista-archive-'));
  t.after(() => rm(directory, { recursive: true, force: true }));

  await mkdir(path.join(directory, 'nested'));
  await writeFile(path.join(directory, 'diagram.txt'), 'diagram');
  await writeFile(path.join(directory, 'nested', 'detail.txt'), 'detail');
  await writeFile(path.join(directory, 'nested', 'skip.txt'), 'skip');

  const matches = file.expand(
    { cwd: directory, nodir: true },
    ['**/*.txt', '!**/skip.txt']
  );
  assert.deepEqual(
    matches.map((name) => name.replaceAll('\\', '/')).sort(),
    ['diagram.txt', 'nested/detail.txt']
  );
  assert.equal(file.expand({ cwd: directory, filter: 'isFile' }, '**/*').length, 3);
});

test('Quasar can still create ZIP archives', async () => {
  const archive = quasarRequire('archiver')('zip', { zlib: { level: 9 } });
  const chunks = [];
  archive.on('data', (chunk) => chunks.push(chunk));
  const completed = once(archive, 'end');

  archive.append('<svg/>', { name: 'diagram.svg' });
  await archive.finalize();
  await completed;

  const output = Buffer.concat(chunks);
  assert.equal(output.readUInt32LE(0), 0x04034b50);
  assert.ok(output.includes(Buffer.from('diagram.svg')));
});

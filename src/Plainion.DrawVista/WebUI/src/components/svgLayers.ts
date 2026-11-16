export type DiagramLayer = {
  id: string;
  name: string;
  visible: boolean;
};

export function readSvgLayers(svg: Element): DiagramLayer[] {
  const version = svg.getAttribute('data-drawvista-layers');
  if (version === null) {
    return [];
  }
  if (version !== '1') {
    throw new Error('Unsupported diagram layer metadata');
  }
  const ids = new Set<string>();
  return Array.from(
    svg.querySelectorAll('[data-drawvista-layer-id]'),
    (group) => {
      const id = group.getAttribute('data-drawvista-layer-id');
      const name = group.getAttribute('data-drawvista-layer-name');
      const visible = group.getAttribute('data-drawvista-layer-visible');
      if (
        group.localName !== 'g' ||
        !id ||
        ids.has(id) ||
        name === null ||
        (visible !== 'true' && visible !== 'false')
      ) {
        throw new Error('Invalid diagram layer metadata');
      }
      ids.add(id);
      return { id, name, visible: visible === 'true' };
    },
  );
}

function preferenceKey(page: string): string {
  return `drawvista.layers:${encodeURIComponent(page)}`;
}

export function restoreLayerPreferences(
  page: string,
  layers: DiagramLayer[],
  storage: Pick<Storage, 'getItem'>,
): DiagramLayer[] {
  const saved = storage.getItem(preferenceKey(page));
  if (saved === null) {
    return layers;
  }
  const entries: unknown = JSON.parse(saved);
  if (!Array.isArray(entries)) {
    throw new Error('Invalid saved layer preferences');
  }
  const preferences = new Map<string, boolean>();
  for (const entry of entries) {
    if (
      !Array.isArray(entry) ||
      entry.length !== 2 ||
      typeof entry[0] !== 'string' ||
      typeof entry[1] !== 'boolean'
    ) {
      throw new Error('Invalid saved layer preferences');
    }
    preferences.set(entry[0], entry[1]);
  }
  return layers.map((layer) => ({
    ...layer,
    visible: preferences.get(layer.id) ?? layer.visible,
  }));
}

export function saveLayerPreferences(
  page: string,
  layers: DiagramLayer[],
  storage: Pick<Storage, 'setItem'>,
): void {
  storage.setItem(
    preferenceKey(page),
    JSON.stringify(layers.map(({ id, visible }) => [id, visible])),
  );
}

export function applyLayerVisibility(
  svg: Element,
  layers: DiagramLayer[],
): void {
  const groups = new Map(
    Array.from(svg.querySelectorAll('[data-drawvista-layer-id]'), (group) => [
      group.getAttribute('data-drawvista-layer-id'),
      group,
    ]),
  );
  for (const layer of layers) {
    const group = groups.get(layer.id);
    if (!group) {
      throw new Error(`Diagram layer is missing: ${layer.id}`);
    }
    group.setAttribute('display', layer.visible ? 'inline' : 'none');
  }
}

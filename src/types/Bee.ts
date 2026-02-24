export type BaseBee = {
  id: string;
  gridX: number;
  gridY: number;
  visualX: number;
  visualY: number;
  targetGridX?: number;
  targetGridY?: number;
  speed: number;
};

export type PollinatorBee = BaseBee & {
  job: "pollinator";
  targetFlower: string | null;
};
export type HarvesterBee = BaseBee & {
  job: "harvester";
  targetHive: string | null;
};
export type IdleBee = BaseBee & { job: "idle" };

export type Bee = PollinatorBee | HarvesterBee | IdleBee;

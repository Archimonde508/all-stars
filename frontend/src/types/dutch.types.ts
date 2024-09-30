export type DutchScore = {
  nickName: string;
  avgPosition: number;
  avgScore: number;
  games: number;
}

export type DutchGame = {
  dutchGameId: string;
  id: string;
  nickName: string;
  points: number;
  position: number;
}

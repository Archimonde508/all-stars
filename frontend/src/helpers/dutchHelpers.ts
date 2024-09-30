import { DutchGame, DutchScore } from "../types/dutch.types";

export const calculateDutchScores = (games: DutchGame[]): DutchScore[] => {
  const groupedByNickName: { [key: string]: DutchGame[] } = games.reduce(
    (acc, game) => {
      if (!acc[game.nickName]) {
        acc[game.nickName] = [];
      }
      acc[game.nickName].push(game);
      return acc;
    },
    {} as { [key: string]: DutchGame[] }
  );

  const dutchScores: DutchScore[] = Object.entries(groupedByNickName).map(
    ([nickName, games]) => {
      const totalPoints = games.reduce((sum, game) => sum + game.points, 0);
      const totalPosition = games.reduce((sum, game) => sum + game.position, 0);
      const avgScore = totalPoints / games.length;
      const avgPosition = totalPosition / games.length;
      return {
        nickName,
        avgPosition: parseFloat(avgPosition.toFixed(1)),
        avgScore: parseFloat(avgScore.toFixed(1)),
        games: games.length,
      };
    }
  );

  const sortedDutchScores = dutchScores.sort((a, b) => a.avgScore - b.avgScore);

  return sortedDutchScores;
};

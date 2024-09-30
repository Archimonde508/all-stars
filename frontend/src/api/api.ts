import { calculateDutchScores } from "../helpers/dutchHelpers";
import { DutchGame, DutchScore } from "../types/dutch.types";
import { Token } from "../types/token.types";

export const login = async (
  login: string,
  password: string
): Promise<Token | null> => {
  try {
    const response = await fetch("http://localhost:5000/login", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        username: login,
        password: password,
      }),
    });

    if (response.status === 401) {
      console.log("nieprawdilowe dane logowania");
      return null;
    }

    if (!response.ok) {
      throw new Error(`Failed to login: ${response.statusText}`);
    }

    const token: Token = await response.json();

    return token;
  } catch (error) {
    console.error("Error when tried to log in:", error);
    return null;
  }
};

type getNickNamesResponse = {
  nickNames: string[]
}

export const getNicknames = async (): Promise<string[]> => {
  try {
    const response = await fetch("http://localhost:5000/users/nicknames", {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        // If authentication is required, pass the JWT token like this:
        // 'Authorization': `Bearer ${yourToken}`
      },
    });

    if (!response.ok) {
      throw new Error(`Error fetching nicknames: ${response.statusText}`);
    }

    const data: getNickNamesResponse = await response.json();
    return data.nickNames;
  } catch (error) {
    console.error("Error fetching nicknames:", error);
    return [];
  }
};

export const getScores = async (): Promise<DutchScore[]> => {
  try {
    const response = await fetch("http://localhost:5000/dutch/all", {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        // If authentication is required, pass the JWT token like this:
        // 'Authorization': `Bearer ${yourToken}`
      },
    });

    if (!response.ok) {
      throw new Error(`Error fetching scores: ${response.statusText}`);
    }

    const data: DutchGame[] = await response.json();
    const scores = calculateDutchScores(data);

    return scores;
  } catch (error) {
    console.error("Error fetching Dutch scores:", error);
    return [];
  }
};

export type CreateDutchGame = {
  scorePairs: { nickName: string; score: number }[];
  comment: string;
};

export const sendScores = async (createDutchGame: CreateDutchGame): Promise<void> => {
  try {
    const response = await fetch("http://localhost:5000/dutch", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        scorePairs: createDutchGame.scorePairs,
        comment: createDutchGame.comment,
      }),
    });

    if (!response.ok) {
      throw new Error(`Error posting scores: ${response.statusText}`);
    }

    console.log("Scores submitted successfully.");
  } catch (error) {
    console.error("Error submitting Dutch scores:", error);
  }
};

import React, { useState, useEffect, useCallback } from "react";
import styled from "styled-components";
import { CreateDutchGame, getNicknames, sendScores } from "../../api/api";

type PlayerRow = {
  nickName: string;
  score: number;
};

type AddGameModalProps = {
  closeModal: () => void;
};


const AddGameModal = ({ closeModal }: AddGameModalProps) => {
  const [playerRows, setPlayerRows] = useState<PlayerRow[]>([{ nickName: "", score: 0 }]);
  const [nicknames, setNicknames] = useState<string[]>([]);

  const addPlayerRow = useCallback(() => {
    setPlayerRows((prevPlayerRows) => [
      ...prevPlayerRows,
      { nickName: "", score: 0 },
    ]);
  }, []);

  useEffect(() => {
    async function fetchNicknames() {
      try {
        const fetchedNicknames = await getNicknames();
        setNicknames(fetchedNicknames);
      } catch (error) {
        console.error("Error fetching nicknames:", error);
      }
    }
    fetchNicknames();
  }, []);

  const removePlayerRow = (index: number) => {
    const newPlayers = playerRows.filter((_, i) => i !== index);
    setPlayerRows(newPlayers);
  };

  const handlePlayerChange = (
    index: number,
    field: keyof PlayerRow,
    value: string
  ) => {
    setPlayerRows((prevPlayers) =>
      prevPlayers.map((player, i) =>
        i === index ? { ...player, [field]: value } : player
      )
    );
  };

  const handleSubmit = async () => {
    const createDutchGame: CreateDutchGame = {
      comment: "placeholder",
      scorePairs: playerRows,
    };
    try {
      sendScores(createDutchGame);
      closeModal();
    } catch (error) {
      console.error("Error saving game:", error);
    }
  };

  return (
    <Modal>
      <ModalContent>
        <ModalHeader>Dodaj Grę</ModalHeader>
        <PlayerRows>
          {playerRows.map((player, index) => {
            const usedNicknames = playerRows
              .filter((_, i) => i !== index)
              .map((p) => p.nickName);

            const availableNicknames = nicknames.filter(
              (nickname) => !usedNicknames.includes(nickname)
            );

            return (
              <PlayerRow key={index}>
                <PlayerRowHeader> Gracz {index + 1} </PlayerRowHeader>
                <Select
                  value={player.nickName}
                  onChange={(e) =>
                    handlePlayerChange(index, "nickName", e.target.value)
                  }
                >
                  <option value="" disabled>
                    Wybierz gracza
                  </option>
                  {availableNicknames.map((nickname) => (
                    <option key={nickname} value={nickname}>
                      {nickname}
                    </option>
                  ))}
                </Select>

                <PointsInput
                  type="number"
                  value={player.score}
                  onChange={(e) =>
                    handlePlayerChange(index, "score", e.target.value)
                  }
                />

                <RemovePlayerButton onClick={() => removePlayerRow(index)}>
                  Usuń
                </RemovePlayerButton>
              </PlayerRow>
            );
          })}
        </PlayerRows>
        <AddPlayerButton onClick={addPlayerRow}>Dodaj gracza</AddPlayerButton>

        <ModalActions>
          <AddPlayerButton onClick={handleSubmit}>Zapisz</AddPlayerButton>
          <AddPlayerButton onClick={closeModal}>Anuluj</AddPlayerButton>
        </ModalActions>
      </ModalContent>
    </Modal>
  );
};

const Modal = styled.div`
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.8);
  display: flex;
  justify-content: center;
  align-items: center;
`;

const ModalContent = styled.div`
  background-color: #1a1a1a;
  padding: 1.875rem;
  border-radius: 0.5rem;
  width: 28.125rem;
  max-width: 90%;
  color: #f5f5f5;
  display: flex;
  flex-direction: column;
  align-items: center;
`;

const PlayerRow = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 0.8rem;
  width: 100%;
`;

const PlayerRows = styled.div`
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  width: 100%;
  margin-top: 0.2rem;
`;

const PointsInput = styled.input`
  width: 3.75rem;
  text-align: center;
  font-size: medium;
  background-color: #2b2b2b;
  color: #f5f5f5;
  border: 1px solid #444;
  padding: 0.3125rem;

  &:focus {
    outline: none;
    border-color: #888;
  }
`;

const Select = styled.select`
  background-color: #2b2b2b;
  color: #f5f5f5;
  border: 1px solid #444;
  padding: 0.5rem;
  width: 11.25rem;

  &:focus {
    outline: none;
    border-color: #888;
  }

  option {
    background-color: #2b2b2b;
    color: #f5f5f5;
  }
`;

const ModalActions = styled.div`
  display: flex;
  justify-content: space-between;
  width: 100%;
  margin-top: 1.25rem;
`;

const AddPlayerButton = styled.button`
  background-color: #444;
  color: #f5f5f5;
  padding: 0.625rem 1.25rem;
  border: none;
  cursor: pointer;
  border-radius: 0.3125rem;
  font-size: 0.875rem;
  transition: background-color 0.3s ease;

  &:hover {
    background-color: #666;
  }

  &:active {
    background-color: #888;
  }
`;

const RemovePlayerButton = styled(AddPlayerButton)`
  background-color: #d9534f;

  &:hover {
    background-color: #c9302c;
  }

  &:active {
    background-color: #ac2925;
  }
`;

const ModalHeader = styled.div`
  margin-bottom: 1.25rem;
  font-size: 1.375rem;
  text-align: center;
  width: 100%;
  font-weight: bold;
`;

const PlayerRowHeader = styled.div`
  font-size: 1.3rem;
`;

export default AddGameModal;

using UnityEngine;
using System.Collections.Generic;

public class Pawn : ChessPiece {
    //pawn movement 
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY) {
        List<Vector2Int> r = new List<Vector2Int>();

        int direction = (team == 0) ? 1 : -1;

        // One in front 
        if (currentY + direction >= 0 && currentY + direction < tileCountY && board[currentX, currentY + direction] == null) {
            r.Add(new Vector2Int(currentX, currentY + direction));
        } 

        // Two in front
        if (currentY == 1 || currentY == 6 && board[currentX, currentY + direction] == null) {
            if(team == 0 && currentY == 1 && board[currentX, currentY + (2 * direction)] == null) {
                r.Add(new Vector2Int(currentX, currentY + (2 * direction)));
            } 
            if (team == 1 && currentY == 6 && board[currentX, currentY + (2 * direction)] == null) {
                r.Add(new Vector2Int(currentX, currentY + (2 * direction)));
            }

        }

        // Kill move 
        if (currentX != tileCountX - 1 && currentY + direction >= 0 && currentY + direction < tileCountY) {
            if (board[currentX + 1, currentY + direction] != null) {
                if (board[currentX + 1, currentY + direction].team != team) {
                    r.Add(new Vector2Int(currentX + 1, currentY + direction));
                }
            }
        } 
        
        if (currentX != 0 && currentY + direction >= 0 && currentY + direction < tileCountY) {
            if (board[currentX - 1, currentY + direction] != null) {
                if (board[currentX - 1, currentY + direction].team != team) {
                    r.Add(new Vector2Int(currentX - 1, currentY + direction));
                }
            }
        }

        return r;
    }
    

    public override SpecialMove GetSpecialMove(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves) {
        
        int direction = (team == 0) ? 1 : -1;
        if ((team == 0 && currentY == 6) || (team == 1 && currentY == 1)) {
           return SpecialMove.Promotion;
        }


        // En passant
        


        if (moveList.Count > 0) {
            Vector2Int[] lastMove = moveList[moveList.Count - 1];

            if (board[lastMove[1].x, lastMove[1].y].type == ChessPieceType.Pawn) { // if the last move is a pawn
                if (Mathf.Abs(lastMove[0].y - lastMove[1].y) == 2) { // Check if the last move was a pawn moving two spaces
                    if (lastMove[1].y == currentY) {
                        if (lastMove[1].x == currentX + 1) { // Check if the pawn is to the right of the current pawn
                            availableMoves.Add(new Vector2Int(currentX + 1, currentY + direction));
                            return SpecialMove.EnPassant;
                        } else if (lastMove[1].x == currentX - 1) {
                            availableMoves.Add(new Vector2Int(currentX - 1, currentY + direction));
                            return SpecialMove.EnPassant;
                        }
                    }
                }
            }
        }

        

        return SpecialMove.None; 

    }
}

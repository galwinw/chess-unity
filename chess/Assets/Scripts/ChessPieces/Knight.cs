using UnityEngine;
using System.Collections.Generic;

public class Knight : ChessPiece {
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY) {
        List<Vector2Int> r = new List<Vector2Int>();

        // Up
        if (currentY + 2 < tileCountY) {
            if (currentX + 1 < tileCountX) {
                if (board[currentX + 1, currentY + 2] == null || board[currentX + 1, currentY + 2].team != team) {
                    r.Add(new Vector2Int(currentX + 1, currentY + 2));
                }
            }
            if (currentX - 1 >= 0) {
                if (board[currentX - 1, currentY + 2] == null || board[currentX - 1, currentY + 2].team != team) {
                    r.Add(new Vector2Int(currentX - 1, currentY + 2));
                }
            }
        }

        // Down
        if (currentY - 2 >= 0) {
            if (currentX + 1 < tileCountX) {
                if (board[currentX + 1, currentY - 2] == null || board[currentX + 1, currentY - 2].team != team) {
                    r.Add(new Vector2Int(currentX + 1, currentY - 2));
                }
            }
            if (currentX - 1 >= 0) {
                if (board[currentX - 1, currentY - 2] == null || board[currentX - 1, currentY - 2].team != team) {
                    r.Add(new Vector2Int(currentX - 1, currentY - 2));
                }
            }
        }

        // Right
        if (currentX + 2 < tileCountX) {
            if (currentY + 1 < tileCountY) {
                if (board[currentX + 2, currentY + 1] == null || board[currentX + 2, currentY + 1].team != team) {
                    r.Add(new Vector2Int(currentX + 2, currentY + 1));
                }
            }
            if (currentY - 1 >= 0) {
                if (board[currentX + 2, currentY - 1] == null || board[currentX + 2, currentY - 1].team != team) {
                    r.Add(new Vector2Int(currentX + 2, currentY - 1));
                }
            }
        }

        // Left
        if (currentX - 2 >= 0) {
            if (currentY + 1 < tileCountY) {
                if (board[currentX - 2, currentY + 1] == null || board[currentX - 2, currentY + 1].team != team) {
                    r.Add(new Vector2Int(currentX - 2, currentY + 1));
                }
            }
            if (currentY - 1 >= 0) {
                if (board[currentX - 2, currentY - 1] == null || board[currentX - 2, currentY - 1].team != team) {
                    r.Add(new Vector2Int(currentX - 2, currentY - 1));
                }
            }
        }

        return r;
    }
}

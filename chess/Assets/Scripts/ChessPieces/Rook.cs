using UnityEngine;
using System.Collections.Generic;

public class Rook : ChessPiece {
    // Rook movemnet
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY) {
        List<Vector2Int> r = new List<Vector2Int>();

        // Up
        for (int i = currentY + 1; i < tileCountY; i++) {
            if (board[currentX, i] == null) {
                r.Add(new Vector2Int(currentX, i));
            } else {
                if (board[currentX, i].team != team) {
                    r.Add(new Vector2Int(currentX, i));
                }
                break;
            }
        }

        // Down
        for (int i = currentY - 1; i >= 0; i--) {
            if (board[currentX, i] == null) {
                r.Add(new Vector2Int(currentX, i));
            } else {
                if (board[currentX, i].team != team) {
                    r.Add(new Vector2Int(currentX, i));
                }
                break;
            }
        }

        // Right
        for (int i = currentX + 1; i < tileCountX; i++) {
            if (board[i, currentY] == null) {
                r.Add(new Vector2Int(i, currentY));
            } else {
                if (board[i, currentY].team != team) {
                    r.Add(new Vector2Int(i, currentY));
                }
                break;
            }
        }

        // Left
        for (int i = currentX - 1; i >= 0; i--) {
            if (board[i, currentY] == null) {
                r.Add(new Vector2Int(i, currentY));
            } else {
                if (board[i, currentY].team != team) {
                    r.Add(new Vector2Int(i, currentY));
                }
                break;
            }
        }

        return r;
    }
}

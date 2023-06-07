using UnityEngine;
using System.Collections.Generic;

public class Bishop : ChessPiece {
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY) {
        List<Vector2Int> r = new List<Vector2Int>();

        // Up-Right
        for (int i = 1; i < tileCountX; i++) {
            if (currentX + i < tileCountX && currentY + i < tileCountY) {
                if (board[currentX + i, currentY + i] == null) {
                    r.Add(new Vector2Int(currentX + i, currentY + i));
                } else if (board[currentX + i, currentY + i].team != team) {
                    r.Add(new Vector2Int(currentX + i, currentY + i));
                    break;
                } else {
                    break;
                }
            } else {
                break;
            }
        }

        // Up-Left
        for (int i = 1; i < tileCountX; i++) {
            if (currentX - i >= 0 && currentY + i < tileCountY) {
                if (board[currentX - i, currentY + i] == null) {
                    r.Add(new Vector2Int(currentX - i, currentY + i));
                } else if (board[currentX - i, currentY + i].team != team) {
                    r.Add(new Vector2Int(currentX - i, currentY + i));
                    break;
                } else {
                    break;
                }
            } else {
                break;
            }
        }

        // Down-Right
        for (int i = 1; i < tileCountX; i++) {
            if (currentX + i < tileCountX && currentY - i >= 0) {
                if (board[currentX + i, currentY - i] == null) {
                    r.Add(new Vector2Int(currentX + i, currentY - i));
                } else if (board[currentX + i, currentY - i].team != team) {
                    r.Add(new Vector2Int(currentX + i, currentY - i));
                    break;
                } else {
                    break;
                }
            } else {
                break;
            }
        }

        // Down-Left
        for (int i = 1; i < tileCountX; i++) {
            if (currentX - i >= 0 && currentY - i >= 0) {
                if (board[currentX - i, currentY - i] == null) {
                    r.Add(new Vector2Int(currentX - i, currentY - i));
                } else if (board[currentX - i, currentY - i].team != team) {
                    r.Add(new Vector2Int(currentX - i, currentY - i));
                    break;
                } else {
                    break;
                }
            } else {
                break;
            }
        }

        return r;
    }
}

using UnityEngine;
using System.Collections.Generic;

public class Queen : ChessPiece {
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

using UnityEngine;
using System.Collections.Generic;

public class King : ChessPiece {
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY) {
        List<Vector2Int> r = new List<Vector2Int>();

        // Up
        if (currentY + 1 < tileCountY) {
            if (board[currentX, currentY + 1] == null || board[currentX, currentY + 1].team != team) {
                r.Add(new Vector2Int(currentX, currentY + 1));
            }
        }

        // Down
        if (currentY - 1 >= 0) {
            if (board[currentX, currentY - 1] == null || board[currentX, currentY - 1].team != team) {
                r.Add(new Vector2Int(currentX, currentY - 1));
            }
        }

        // Right
        if (currentX + 1 < tileCountX) {
            if (board[currentX + 1, currentY] == null || board[currentX + 1, currentY].team != team) {
                r.Add(new Vector2Int(currentX + 1, currentY));
            }
        }

        // Left
        if (currentX - 1 >= 0) {
            if (board[currentX - 1, currentY] == null || board[currentX - 1, currentY].team != team) {
                r.Add(new Vector2Int(currentX - 1, currentY));
            }
        }

        // Up-Right
        if (currentX + 1 < tileCountX && currentY + 1 < tileCountY) {
            if (board[currentX + 1, currentY + 1] == null || board[currentX + 1, currentY + 1].team != team) {
                r.Add(new Vector2Int(currentX + 1, currentY + 1));
            }
        }

        // Up-Left
        if (currentX - 1 >= 0 && currentY + 1 < tileCountY) {
            if (board[currentX - 1, currentY + 1] == null || board[currentX - 1, currentY + 1].team != team) {
                r.Add(new Vector2Int(currentX - 1, currentY + 1));
            }
        }

        // Down-Right
        if (currentX + 1 < tileCountX && currentY - 1 >= 0) {
            if (board[currentX + 1, currentY - 1] == null || board[currentX + 1, currentY - 1].team != team) {
                r.Add(new Vector2Int(currentX + 1, currentY - 1));
            }
        }

        // Down-Left
        if (currentX - 1 >= 0 && currentY - 1 >= 0) {
            if (board[currentX - 1, currentY - 1] == null || board[currentX - 1, currentY - 1].team != team) {
                r.Add(new Vector2Int(currentX - 1, currentY - 1));
            }
        }

        return r;
    }

    public override SpecialMove GetSpecialMove(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves) {
        // Castling
        SpecialMove r = SpecialMove.None;

        var kingMove = moveList.Find(m => m[0].x == 4 && m[0].y == ((team == 0) ? 0 : 7));
        var leftRook = moveList.Find(m => m[0].x == 0 && m[0].y == ((team == 0) ? 0 : 7));
        var rightRook = moveList.Find(m => m[0].x == 7 && m[0].y == ((team == 0) ? 0 : 7));

        if (kingMove == null && currentX == 4) {
            // White 
            if (team == 0) {
                // left rook
                if (leftRook == null) {
                    if (board[0,0].type == ChessPieceType.Rook) {
                        if (board[0,0].team == 0) {
                            if (board[3,0] == null && board[2,0] == null && board[1,0] == null) {
                                r = SpecialMove.Castling;
                                availableMoves.Add(new Vector2Int(2, 0));
                            }
                        }
                    }
                }

                //Right rook
                if (rightRook == null) {
                    if (board[7,0].type == ChessPieceType.Rook) {
                        if (board[7,0].team == 0) {
                            if (board[5,0] == null && board[6,0] == null) {
                                r = SpecialMove.Castling;
                                availableMoves.Add(new Vector2Int(6, 0));
                            }
                        }
                    }
                }
            } else {
                // left rook
                if (leftRook == null) {
                    if (board[0,7].type == ChessPieceType.Rook) {
                        if (board[0,7].team == 1) {
                            if (board[3,7] == null && board[2,7] == null && board[1,7] == null) {
                                r = SpecialMove.Castling;
                                availableMoves.Add(new Vector2Int(2, 7));
                            }
                        }
                    }
                }

                //Right rook
                if (rightRook == null) {
                    if (board[7,7].type == ChessPieceType.Rook) {
                        if (board[7,7].team == 1) {
                            if (board[5,7] == null && board[6,7] == null) {
                                r = SpecialMove.Castling;
                                availableMoves.Add(new Vector2Int(6, 7));
                            }
                        }
                    }
                }
            }
        }



        return r; 
    }   
}

namespace minegamedis {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Mines {
        // General variable declaration
        private bool revealed = false;
        private bool bomb = false;
        private bool marked = false;
        private bool pmarked = false;
        private bool greenbonus = false;
        private bool purplebonus = false;
        private bool specialbonus = false;
        private bool wide = false;
        private bool touching = false;
        private int tLeft, tRight, bLeft, bRight;
        private int nearbyBomb = 0;
        private int x, y;

        public Mines(int x, int y) {
            this.x = x;
            this.y = y;
        }

        // Count neighbour mines for juicedsweeper
        public void CountMines(int w, Mines[,] mine) {
            if (this.bomb) {
                this.nearbyBomb = -1;
            } else {
                int count = 0;
                for (int xoffset = -1; xoffset <= 1; xoffset++) {
                    int x1 = x + xoffset;
                    if (x1 < 0 || x1 >= w) continue;
                    for (int yoffset = -1; yoffset <= 1; yoffset++) {
                        int y1 = y + yoffset;
                        if (y1 < 0 || y1 >= w) continue;
                        Mines mine2 = mine[x1, y1];
                        if (mine2.bomb) {
                            count++;
                        }
                    }
                }
                this.nearbyBomb = count;
            }
        }

        // Counting touching neighbour bombs for bracker tiles
        public void CountTouch(int w, Mines[,] mine) {
            int temp = 0;
            bool tempt = true;
            for (int xoffset = -1; xoffset <= 0; xoffset++) {
                int x1 = x + xoffset;
                if (x1 < 0 || x1 >= w) continue;
                for (int yoffset = 0; yoffset <= 1; yoffset++) {
                    int y1 = y + yoffset;
                    if (y1 < 0 || y1 >= w) continue;
                    Mines mine2 = mine[x1, y1];
                    if (tempt) {
                        if (yoffset == 1 && temp == 1 && !mine2.bomb) {
                            tempt = false;
                        } else if (mine2.bomb) {
                            temp++;
                        }
                    }
                }
            }
            if (temp >= 2 && tempt) {
                touching = true;
                this.tLeft = temp;
            }
            temp = 0;
            tempt = true;
            for (int xoffset = 0; xoffset <= 1; xoffset++) {
                int x1 = x + xoffset;
                if (x1 < 0 || x1 >= w) continue;
                for (int yoffset = 1; yoffset >= 0; yoffset--) {
                    int y1 = y + yoffset;
                    if (y1 < 0 || y1 >= w) continue;
                    Mines mine2 = mine[x1, y1];
                    if (tempt) {
                        if (xoffset == 1 && temp == 1 && !mine2.bomb) {
                            tempt = false;
                        } else if (mine2.bomb) {
                            temp++;
                        }
                    }
                }
            }
            if (temp >= 2 && tempt) {
                touching = true;
                this.tRight = temp;
            }
            temp = 0;
            tempt = true;
            for (int xoffset = -1; xoffset <= 0; xoffset++) {
                int x1 = x + xoffset;
                if (x1 < 0 || x1 >= w) continue;
                for (int yoffset = 0; yoffset >= -1; yoffset--) {
                    int y1 = y + yoffset;
                    if (y1 < 0 || y1 >= w) continue;
                    Mines mine2 = mine[x1, y1];
                    if(tempt) {
                        if (yoffset == -1 && temp == 1 && !mine2.bomb) {
                            tempt = false;
                        } else if (mine2.bomb) {
                            temp++;
                        }
                    } 
                }
            }
            if (temp >= 2 && tempt) {
                touching = true;
                this.bLeft = temp;
            }
            temp = 0;
            tempt = true;
            for (int xoffset = 1; xoffset >= 0; xoffset--) {
                int x1 = x + xoffset;
                if (x1 < 0 || x1 >= w) continue;
                for (int yoffset = 0; yoffset >= -1; yoffset--) {
                    int y1 = y + yoffset;
                    if (y1 < 0 || y1 >= w) continue;
                    Mines mine2 = mine[x1, y1];
                    if (tempt) {
                        if (yoffset == -1 && temp == 1 && !mine2.bomb) {
                            tempt = false;
                        } else if (mine2.bomb) {
                            temp++;
                        }
                    }
                }
            }
            if (temp >= 2 && tempt) {
                touching = true;
                this.bRight = temp;
            }
        }

        // Get method for neighbour bomb counter
        public int GetCount(int type) {
            if (type == 0) {
                return this.tLeft;
            } else if (type == 1) {
                return this.tRight;
            } else if (type == 2) {
                return this.bLeft;
            } else {
                return this.bRight;
            }
        }


        // Reveal tiles left and right of double arrow tile
        public void RevealWide(int w, Mines[,] mine) {
            this.revealed = true;
            Mines mine2;

            mine2 = mine[x - 1, y];
            mine2.SetRevealed();
            mine2 = mine[x + 1, y];
            mine2.SetRevealed(); 
        }

        // Reveal method for normal minesweeper using floodfill
        public void IsRevealed(int w, Mines[,] mine) {
            this.revealed = true;
            if (this.nearbyBomb == 0) {
                this.floodFill(w, mine);
            }
        }

        // Floodfill method for normal minesweeper
        public void floodFill(int w, Mines[,] mine) {
            for (int xoffset = -1; xoffset <= 1; xoffset++) {
                int x1 = x + xoffset;
                if (x1 < 0 || x1 >= w) continue;
                for (int yoffset = -1; yoffset <= 1; yoffset++) {
                    int y1 = y + yoffset;
                    if (y1 < 0 || y1 >= w) continue;
                    Mines mine2 = mine[x1, y1];
                    if (!mine2.revealed) {
                        mine2.IsRevealed(w, mine);
                    }
                }
            }
        }


        // Normal reveal method for juicedsweeper
        public void SetRevealed() {
            this.revealed = true;
        }

        // Set method for all bonus tiles
        public void SetBonus(int type) {
            if (type == 0) {
                this.greenbonus = true;
            } else if (type == 1) {
                this.purplebonus = true;
            } else if (type == 2) {
                this.wide = true;
            } else {
                this.specialbonus = true;
            }
        }

        // Get method for all bonus tiles
        public bool GetBonus(int type) {
            if (type == 0) {
                return this.greenbonus;
            } else if (type == 1) {
                return this.purplebonus;
            } else if (type == 2) {
                return this.wide;
            } else {
                return this.specialbonus;
            }
        }

        // Series of get and set methods for the variables

        public void SetMarked() {
            this.marked = !marked;
        }

        public bool GetMarked() {
            return marked;
        }
        
        public bool GetTouch() {
            return touching;
        }

        public void SetNearbyBomb(int i) {
            this.nearbyBomb = i;
        }

        public int GetNearbyBomb() {
            return nearbyBomb;
        }

        public bool GetRevealed() {
            return revealed;
        }

        public void SetBomb() {
            this.bomb = true;
        }

        public bool GetBomb() {
            return bomb;
        }

        public void SetWide() {
            this.wide = true;
        }

        public bool GetWide() {
            return wide;
        }

        public void SetPMark() {
            this.pmarked = !pmarked;
        }

        public bool GetPMark() {
            return this.pmarked;
        }

    }
    
}
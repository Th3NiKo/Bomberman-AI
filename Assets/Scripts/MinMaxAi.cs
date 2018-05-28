using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Możliwości: 
// idź - koszt -1 (woda koszt - 10) (do przeciwnika +5)
// obrót w lewo - koszt -1
// obrót w prawo - koszt -1
// w/w oblicza a* 
// czekaj - koszt 0 (bomba -100)
// postaw bombę - koszt -5 (przy przeciwniku + 20)

// pola:
// bomba i kratka w lewo, prawo, gora, dół, d = -1000
// puste
// przeciwnicy
// woda

// na poczatku kazdej tury sprawdzic czy jest przejscie do przeciwnikow
// jesli nie ma, rozwalac klocki do najblizszego
// jesli jest przescie do jakiegos - atak
// sprawdzac czy jest bomba 2 kratki dalej
// jesli jest zawroc, jesli nie atak

public class MinMaxAi : MonoBehaviour {

    GameManager gameManager;
    public int playerIndex = 30;
    int[,] map;

    //Timers
    float timer = 0.0f;
    float turnTime;

	// Use this for initialization
	void Start () {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        turnTime = gameManager.GetTurnTime();
		
	}

    int[] MinMax () {
        //gameManager.players[2] x, y, orientation
        List<Action> queue;
        map = gameManager.GetMap();
        int positionX = gameManager.players[2].x;
        int positionY = gameManager.players[2].y;
        Direction orientation = gameManager.players[2].Orientation;

        int[] listaPriorytetow = new int [5] {0, 0, 0, 0, 0};

        //obok wood, sciana, woda, obok
        /* PO LEWEJ */
        if (map[positionX-1, positionY] == 8 || map[positionX-1, positionY] == 6 || map[positionX-1, positionY] == 9) {
            listaPriorytetow[0] += 0;
        }
        /* Na GÓRZE */
        if (map[positionX, positionY+1] == 8 || map[positionX, positionY+1] == 6 || map[positionX, positionY+1] == 9) {
            listaPriorytetow[0] += 0;
        }
        /* PO PRAWEJ */
        if (map[positionX+1, positionY] == 8 || map[positionX+1, positionY] == 6 || map[positionX+1, positionY] == 9) {
            listaPriorytetow[0] += 0;
        }
        /* NA DOLE */
        if (map[positionX, positionY-1] == 8 || map[positionX, positionY-1] == 6 || map[positionX, positionY-1] == 9) {
            listaPriorytetow[0] += 0;
        }

        //obok bomby
        /* PO LEWEJ */
        if (map[positionX-1, positionY] > 1 && map[positionX-1, positionY] < 5) {
            listaPriorytetow[1] -= 100;
        }
        /* Na GÓRZE */
        if (map[positionX, positionY+1] > 1 && map[positionX, positionY+1] < 5) {
            listaPriorytetow[1] -= 100;
        }
        /* PO PRAWEJ */
        if (map[positionX+1, positionY] > 1 && map[positionX+1, positionY] < 5) {
            listaPriorytetow[1] -= 100;
        }
        /* NA DOLE */
        if (map[positionX, positionY-1] > 1 && map[positionX, positionY-1] < 5) {
            listaPriorytetow[1] -= 100;
        }

        //obok przeciwnik
        /* PO LEWEJ */
        if (map[positionX-1, positionY] == 10 || map[positionX-1, positionY] == 20) {
            listaPriorytetow[2] += 5;
        }
        /* Na GÓRZE */
        if (map[positionX, positionY+1] == 10 || map[positionX, positionY+1] == 20) {
            listaPriorytetow[2] += 5;
        }
        /* PO PRAWEJ */
        if (map[positionX+1, positionY] == 10 || map[positionX+1, positionY] == 20) {
            listaPriorytetow[2] += 5;
        }
        /* NA DOLE */
        if (map[positionX, positionY-1] == 10 || map[positionX, positionY-1] == 20) {
            listaPriorytetow[2] += 5;
        }

        //puste pole 3 warianty
        //puste i obok bomba
        /* PO LEWEJ */
        if (map[positionX-1, positionY] == 0) {
            if( (map[positionX-1, positionY-1] > 1 && map[positionX-1, positionY-1] < 5) || 
                (map[positionX-2, positionY] > 1 && map[positionX-2, positionY] < 5) ||
                (map[positionX-1, positionY+1] > 1 && map[positionX-1, positionY+1] < 5) ) {
                listaPriorytetow[3] -= 100;
            }
        }
        /* Na GÓRZE */
        if (map[positionX, positionY+1] == 0) {
            if( (map[positionX, positionY+2] > 1 && map[positionX, positionY+2] < 5) || 
                (map[positionX+1, positionY+1] > 1 && map[positionX+1, positionY+1] < 5) ) {
                listaPriorytetow[3] -= 100;
            }
        }
        /* PO PRAWEJ */
        if (map[positionX+1, positionY] == 0) {
            if( (map[positionX+2, positionY] > 1 && map[positionX+2, positionY] < 5) || 
                (map[positionX+1, positionY-1] > 1 && map[positionX+1, positionY-1] < 5) ) {
                listaPriorytetow[3] -= 100;
            }
        }
        /* NA DOLE */
        if (map[positionX, positionY-1] == 0) {
            if( (map[positionX, positionY-2] > 1 && map[positionX, positionY-2] < 5) ) {
                listaPriorytetow[3] -= 100;
            }
        }

        //puste i obok player
        /* PO LEWEJ */
        if (map[positionX-1, positionY] == 0) {
            if( (map[positionX-1, positionY-1] == 10 || map[positionX-1, positionY-1] == 20) || 
                (map[positionX-2, positionY] == 10 || map[positionX-2, positionY] == 20) ||
                (map[positionX-1, positionY+1] == 10 || map[positionX-1, positionY+1] == 20) ) {
                listaPriorytetow[4] += 10;
            }
        }
        /* Na GÓRZE */
        if (map[positionX, positionY+1] == 0) {
            if( (map[positionX, positionY+2] == 10 || map[positionX, positionY+2] == 20) || 
                (map[positionX+1, positionY+1] == 10 || map[positionX+1, positionY+1] == 20) ) {
                listaPriorytetow[4] += 10;
            }
        }
        /* PO PRAWEJ */
        if (map[positionX+1, positionY] == 0) {
            if( (map[positionX+2, positionY] == 10 || map[positionX+2, positionY] == 20) || 
                (map[positionX+1, positionY-1] == 10 || map[positionX+1, positionY-1] == 20) ) {
                listaPriorytetow[4] += 10;
            }
        }
        /* NA DOLE */
        if (map[positionX, positionY-1] == 0) {
            if( (map[positionX, positionY-2] == 10 || map[positionX, positionY-2] == 20) ) {
                listaPriorytetow[4] += 10;
            }
        }

        return listaPriorytetow;

    }

    int Move (int index) {
        map = gameManager.GetMap();
        Action ruch;
        switch(index) {
            //podchodzimy do gracza
            case 0:
                int iloscRuchow = 100;
                List<Action> listaKrokow;
                //gracz 1
                if (map[gameManager.players[0].x-1, gameManager.players[0].y] == 0) {
                    listaKrokow = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y), 
                                                                gameManager.players[2].Orientation, 
                                                                new Vector2(gameManager.players[0].x-1, gameManager.players[0].y),
                                                                gameManager.players[2].Orientation);
                    if  (listaKrokow != null && iloscRuchow > listaKrokow.Count) {
                        iloscRuchow = listaKrokow.Count;
                        ruch = listaKrokow[0];
                    }
                }
                if (map[gameManager.players[0].x, gameManager.players[0].y+1] == 0) {
                    listaKrokow = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y), 
                                                                gameManager.players[2].Orientation, 
                                                                new Vector2(gameManager.players[0].x, gameManager.players[0].y+1),
                                                                gameManager.players[2].Orientation);
                    if  (listaKrokow != null && iloscRuchow > listaKrokow.Count) {
                        iloscRuchow = listaKrokow.Count;
                        ruch = listaKrokow[0];
                    }
                }
                if (map[gameManager.players[0].x+1, gameManager.players[0].y] == 0) {
                    listaKrokow = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y), 
                                                                gameManager.players[2].Orientation, 
                                                                new Vector2(gameManager.players[0].x+1, gameManager.players[0].y),
                                                                gameManager.players[2].Orientation);
                    if  (listaKrokow != null && iloscRuchow > listaKrokow.Count) {
                        iloscRuchow = listaKrokow.Count;
                        ruch = listaKrokow[0];
                    }
                }
                if (map[gameManager.players[0].x, gameManager.players[0].y-1] == 0) {
                    listaKrokow = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y), 
                                                                gameManager.players[2].Orientation, 
                                                                new Vector2(gameManager.players[0].x, gameManager.players[0].y-1),
                                                                gameManager.players[2].Orientation);
                    if  (listaKrokow != null && iloscRuchow > listaKrokow.Count) {
                        iloscRuchow = listaKrokow.Count;
                        ruch = listaKrokow[0];
                    }
                }
                //Gracz 2
                if (map[gameManager.players[1].x-1, gameManager.players[1].y] == 0) {
                    listaKrokow = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y), 
                                                                gameManager.players[2].Orientation, 
                                                                new Vector2(gameManager.players[1].x-1, gameManager.players[1].y),
                                                                gameManager.players[2].Orientation);
                    if  (listaKrokow != null && iloscRuchow > listaKrokow.Count) {
                        iloscRuchow = listaKrokow.Count;
                        ruch = listaKrokow[0];
                    }
                }
                if (map[gameManager.players[1].x, gameManager.players[1].y+1] == 0) {
                    listaKrokow = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y), 
                                                                gameManager.players[2].Orientation, 
                                                                new Vector2(gameManager.players[1].x, gameManager.players[1].y+1),
                                                                gameManager.players[2].Orientation);
                    if  (listaKrokow != null && iloscRuchow > listaKrokow.Count) {
                        iloscRuchow = listaKrokow.Count;
                        ruch = listaKrokow[0];
                    }
                }
                if (map[gameManager.players[1].x+1, gameManager.players[1].y] == 0) {
                    listaKrokow = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y), 
                                                                gameManager.players[2].Orientation, 
                                                                new Vector2(gameManager.players[1].x+1, gameManager.players[1].y),
                                                                gameManager.players[2].Orientation);
                    if  (listaKrokow != null && iloscRuchow > listaKrokow.Count) {
                        iloscRuchow = listaKrokow.Count;
                        ruch = listaKrokow[0];
                    }
                }
                if (map[gameManager.players[1].x, gameManager.players[1].y-1] == 0) {
                    listaKrokow = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y), 
                                                                gameManager.players[2].Orientation, 
                                                                new Vector2(gameManager.players[1].x, gameManager.players[1].y-1),
                                                                gameManager.players[2].Orientation);
                    if  (listaKrokow != null && iloscRuchow > listaKrokow.Count) {
                        iloscRuchow = listaKrokow.Count;
                        ruch = listaKrokow[0];
                    }
                }
                //jesli nie mamy podejscia to czekamy
                if (iloscRuchow == 100) {
                    ruch = null;
                }
            break;
            
            case 1:
            case 2:
                int iloscRuchow = 100;
                List<Action> listaKrokow;
                if (map[gameManager.players[2].x-1, gameManager.players[2].y] == 0) {
                    listaKrokow = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y), 
                                                                gameManager.players[2].Orientation, 
                                                                new Vector2(gameManager.players[2].x-1, gameManager.players[2].y),
                                                                Orientation.LEFT);
                    if  (listaKrokow != null && iloscRuchow > listaKrokow.Count) {
                        iloscRuchow = listaKrokow.Count;
                        ruch = listaKrokow[0];
                    }
                }
                if (map[gameManager.players[2].x, gameManager.players[2].y+1] == 0) {
                    listaKrokow = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y), 
                                                                gameManager.players[2].Orientation, 
                                                                new Vector2(gameManager.players[2].x, gameManager.players[2].y+1),
                                                                Orientation.UP);
                    if  (listaKrokow != null && iloscRuchow > listaKrokow.Count) {
                        iloscRuchow = listaKrokow.Count;
                        ruch = listaKrokow[0];
                    }
                }
                if (map[gameManager.players[2].x+1, gameManager.players[2].y] == 0) {
                    listaKrokow = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y), 
                                                                gameManager.players[2].Orientation, 
                                                                new Vector2(gameManager.players[2].x+1, gameManager.players[2].y),
                                                                Orientation.RIGHT);
                    if  (listaKrokow != null && iloscRuchow > listaKrokow.Count) {
                        iloscRuchow = listaKrokow.Count;
                        ruch = listaKrokow[0];
                    }
                }
                if (map[gameManager.players[2].x, gameManager.players[2].y-1] == 0) {
                    listaKrokow = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y), 
                                                                gameManager.players[2].Orientation, 
                                                                new Vector2(gameManager.players[2].x, gameManager.players[2].y-1),
                                                                Orientation.DOWN);
                    if  (listaKrokow != null && iloscRuchow > listaKrokow.Count) {
                        iloscRuchow = listaKrokow.Count;
                        ruch = listaKrokow[0];
                    }
                }
                //jesli nie mozemy uciec od bomby
                if (iloscRuchow == 100){
                    ruch = null;
                }
            break;

            case 3:
                int iloscRuchow = 100;
                List<Action> listaKrokow;
                
            break;

            case 4:
                int iloscRuchow = 100;
                List<Action> listaKrokow;
                //lewo
                if (map[positionX-1, positionY] == 0) {
                    if( (map[positionX-1, positionY-1] == 10 || map[positionX-1, positionY-1] == 20) || 
                        (map[positionX-2, positionY] == 10 || map[positionX-2, positionY] == 20) ||
                        (map[positionX-1, positionY+1] == 10 || map[positionX-1, positionY+1] == 20) ) {
                        if (gameManager.players[2].Orientation == Orientation.LEFT) {
                            gameManager.PlaceBomb(playerIndex);
                            ruch = null;
                        } else {
                            listaKrokow = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y), 
                                                                    gameManager.players[2].Orientation, 
                                                                    new Vector2(gameManager.players[2].x, gameManager.players[2].y),
                                                                    Orientation.LEFT);

                            if  (listaKrokow != null && iloscRuchow > listaKrokow.Count) {
                                iloscRuchow = listaKrokow.Count;
                                ruch = listaKrokow[0];
                            }
                        }
                        
                    }
                }
                //gora
                if (map[positionX, positionY+1] == 0) {
                    if( (map[positionX-1, positionY+1] == 10 || map[positionX-1, positionY+1] == 20) || 
                        (map[positionX, positionY+2] == 10 || map[positionX, positionY+2] == 20) ||
                        (map[positionX+1, positionY+1] == 10 || map[positionX+1, positionY+1] == 20) ) {
                        if (gameManager.players[2].Orientation == Orientation.UP) {
                            gameManager.PlaceBomb(playerIndex);
                            ruch = null;
                        } else {
                            listaKrokow = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y), 
                                                                    gameManager.players[2].Orientation, 
                                                                    new Vector2(gameManager.players[2].x, gameManager.players[2].y),
                                                                    Orientation.UP);

                            if  (listaKrokow != null && iloscRuchow > listaKrokow.Count) {
                                iloscRuchow = listaKrokow.Count;
                                ruch = listaKrokow[0];
                            }
                        }
                        
                    }
                }
                //prawo
                if (map[positionX+1, positionY] == 0) {
                    if( (map[positionX+1, positionY+1] == 10 || map[positionX+1, positionY+1] == 20) || 
                        (map[positionX+2, positionY] == 10 || map[positionX+2, positionY] == 20) ||
                        (map[positionX+1, positionY-1] == 10 || map[positionX+1, positionY-1] == 20) ) {
                        if (gameManager.players[2].Orientation == Orientation.RIGHT) {
                            gameManager.PlaceBomb(playerIndex);
                            ruch = null;
                        } else {
                            listaKrokow = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y), 
                                                                    gameManager.players[2].Orientation, 
                                                                    new Vector2(gameManager.players[2].x, gameManager.players[2].y),
                                                                    Orientation.RIGHT);

                            if  (listaKrokow != null && iloscRuchow > listaKrokow.Count) {
                                iloscRuchow = listaKrokow.Count;
                                ruch = listaKrokow[0];
                            }
                        }
                        
                    }
                }
                //dol
                if (map[positionX, positionY-1] == 0) {
                    if( (map[positionX+1, positionY-1] == 10 || map[positionX+1, positionY-1] == 20) || 
                        (map[positionX, positionY-2] == 10 || map[positionX, positionY-2] == 20) ||
                        (map[positionX-1, positionY-1] == 10 || map[positionX-1, positionY-1] == 20) ) {
                        if (gameManager.players[2].Orientation == Orientation.RIGHT) {
                            gameManager.PlaceBomb(playerIndex);
                            ruch = null;
                        } else {
                            listaKrokow = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y), 
                                                                    gameManager.players[2].Orientation, 
                                                                    new Vector2(gameManager.players[2].x, gameManager.players[2].y),
                                                                    Orientation.RIGHT);

                            if  (listaKrokow != null && iloscRuchow > listaKrokow.Count) {
                                iloscRuchow = listaKrokow.Count;
                                ruch = listaKrokow[0];
                            }
                        }
                        
                    }
                }
            break;
        }

        switch(ruch){
            case null:
                return -1;
            break;

            case Action.MoveForward:
                gameManager.MoveForward(playerIndex);
                return -1;
            break;

            case Action.RotateClockwise:
                gameManager.RotateClockwise(playerIndex);
                return -1;
            break;

            case Action.RotateCounterClockwise:
                gameManager.RotateCounterClockwise(playerIndex);
                return -1;
            break;
        }
    }
	
	// Update is called once per frame
	void Update () {
        int max0;
        int max1 = -1000;
        int max2 = -1000;
        int max3 = -1000;
        int max4 = -1000;
        if (timer > turnTime) {
            
            int[] listaPriorytetow = MinMax();

            int[] listaPriorytetowIndex = new int [5];
            for (int i=0; i<listaPriorytetow.Length; i++) {
                listaPriorytetowIndex[i] = listaPriorytetow[i];
            }

            Array.Sort<int>(listaPriorytetowIndex);

            
            int tmp = 1;
            int licznik = 4;
            while(tmp) {
                int wartosc = listaPriorytetowIndex[licznik];
                for (int i=0; i<listaPriorytetow.Length; i++) {
                    listaPriorytetow[i] == wartosc;
                    tmp = Move(i);
                    break;
                }
                licznik--;
            }
            


            timer = 0.0f;
        }
        timer += Time.deltaTime;
	}
}

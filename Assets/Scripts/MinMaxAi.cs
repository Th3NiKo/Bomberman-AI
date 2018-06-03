﻿using System.Collections;
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

public class MinMaxAi : MonoBehaviour
{

    GameManager gameManager;
    public int playerIndex = 30;
    int[,] map;

    //Timers
    float timer = 0.0f;
    float turnTime;

    // Use this for initialization
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        turnTime = gameManager.GetTurnTime();

    }

    int[] MinMax()
    {
        //gameManager.players[2] x, y, orientation
        List<Action> queue;
        map = gameManager.GetMap();
        int positionX = gameManager.players[2].x;
        int positionY = gameManager.players[2].y;
        // int bombaX;
        // int bombaY;
        Direction orientation = gameManager.players[2].Orientation;

        int[] listaPriorytetow = new int[6] { 0, 0, 0, 0, 0, 0 };

        //obok wood, sciana, woda, obok
        /* PO LEWEJ */
        if (map[positionX - 1, positionY] == 8 || map[positionX - 1, positionY] == 6 || map[positionX - 1, positionY] == 9)
        {
            listaPriorytetow[0] += 0;
        }
        /* Na GÓRZE */
        if (map[positionX, positionY + 1] == 8 || map[positionX, positionY + 1] == 6 || map[positionX, positionY + 1] == 9)
        {
            listaPriorytetow[0] += 0;
        }
        /* PO PRAWEJ */
        if (map[positionX + 1, positionY] == 8 || map[positionX + 1, positionY] == 6 || map[positionX + 1, positionY] == 9)
        {
            listaPriorytetow[0] += 0;
        }
        /* NA DOLE */
        if (map[positionX, positionY - 1] == 8 || map[positionX, positionY - 1] == 6 || map[positionX, positionY - 1] == 9)
        {
            listaPriorytetow[0] += 0;
        }

        //obok bomby
        /* PO LEWEJ */
        if (map[positionX - 1, positionY] > 0 && map[positionX - 1, positionY] < 5)
        {
            listaPriorytetow[1] += 100;
        }
        /* Na GÓRZE */
        if (map[positionX, positionY + 1] > 0 && map[positionX, positionY + 1] < 5)
        {
            listaPriorytetow[1] += 100;
        }
        /* PO PRAWEJ */
        if (map[positionX + 1, positionY] > 0 && map[positionX + 1, positionY] < 5)
        {
            listaPriorytetow[1] += 100;
        }
        /* NA DOLE */
        if (map[positionX, positionY - 1] > 0 && map[positionX, positionY - 1] < 5)
        {
            listaPriorytetow[1] += 100;
        }

        //obok przeciwnik
        /* PO LEWEJ */
        if (map[positionX - 1, positionY] == 10 || map[positionX - 1, positionY] == 20)
        {
            listaPriorytetow[2] += 5;
        }
        /* Na GÓRZE */
        if (map[positionX, positionY + 1] == 10 || map[positionX, positionY + 1] == 20)
        {
            listaPriorytetow[2] += 5;
        }
        /* PO PRAWEJ */
        if (map[positionX + 1, positionY] == 10 || map[positionX + 1, positionY] == 20)
        {
            listaPriorytetow[2] += 5;
        }
        /* NA DOLE */
        if (map[positionX, positionY - 1] == 10 || map[positionX, positionY - 1] == 20)
        {
            listaPriorytetow[2] += 5;
        }

        //puste pole 3 warianty
        //puste i obok bomba
        /* PO LEWEJ */
        if (map[positionX - 1, positionY] == 0)
        {
            if ((map[positionX - 1, positionY - 1] > 0 && map[positionX - 1, positionY - 1] < 5) ||
                (map[positionX - 2, positionY] > 0 && map[positionX - 2, positionY] < 5) ||
                (map[positionX - 1, positionY + 1] > 0 && map[positionX - 1, positionY + 1] < 5))
            {
                listaPriorytetow[3] += 90;
            }
        }
        /* Na GÓRZE */
        if (map[positionX, positionY + 1] == 0)
        {
            if ((map[positionX, positionY + 2] > 0 && map[positionX, positionY + 2] < 5) ||
                (map[positionX + 1, positionY + 1] > 0 && map[positionX + 1, positionY + 1] < 5))
            {
                listaPriorytetow[3] += 90;
            }
        }
        /* PO PRAWEJ */
        if (map[positionX + 1, positionY] == 0)
        {
            if ((map[positionX + 2, positionY] > 0 && map[positionX + 2, positionY] < 5) ||
                (map[positionX + 1, positionY - 1] > 0 && map[positionX + 1, positionY - 1] < 5))
            {
                listaPriorytetow[3] += 90;
            }
        }
        /* NA DOLE */
        if (map[positionX, positionY - 1] == 0)
        {
            if ((map[positionX, positionY - 2] > 0 && map[positionX, positionY - 2] < 5))
            {
                listaPriorytetow[3] += 90;
            }
        }

        //puste i obok player
        /* PO LEWEJ */
        if (map[positionX - 1, positionY] == 0)
        {
            if ((map[positionX - 1, positionY - 1] == 10 || map[positionX - 1, positionY - 1] == 20) ||
                (map[positionX - 2, positionY] == 10 || map[positionX - 2, positionY] == 20) ||
                (map[positionX - 1, positionY + 1] == 10 || map[positionX - 1, positionY + 1] == 20))
            {
                listaPriorytetow[4] += 10;
            }
        }
        /* Na GÓRZE */
        if (map[positionX, positionY + 1] == 0)
        {
            if ((map[positionX, positionY + 2] == 10 || map[positionX, positionY + 2] == 20) ||
                (map[positionX + 1, positionY + 1] == 10 || map[positionX + 1, positionY + 1] == 20))
            {
                listaPriorytetow[4] += 10;
            }
        }
        /* PO PRAWEJ */
        if (map[positionX + 1, positionY] == 0)
        {
            if ((map[positionX + 2, positionY] == 10 || map[positionX + 2, positionY] == 20) ||
                (map[positionX + 1, positionY - 1] == 10 || map[positionX + 1, positionY - 1] == 20))
            {
                listaPriorytetow[4] += 10;
            }
        }
        /* NA DOLE */
        if (map[positionX, positionY - 1] == 0)
        {
            if ((map[positionX, positionY - 2] == 10 || map[positionX, positionY - 2] == 20))
            {
                listaPriorytetow[4] += 10;
            }
        }
        // skierowani na puste pole oraz przeciwnik jest skierowany na to samo puste pole = postaw bombe
        switch (orientation)
        {
            case Direction.UP:
                if (map[positionX, positionY + 1] == 0)
                {
                    if ((map[positionX - 1, positionY + 1] == 20 && gameManager.players[1].Orientation == Direction.RIGHT)
                    || (map[positionX - 1, positionY + 1] == 10 && gameManager.players[0].Orientation == Direction.RIGHT))
                    {
                        listaPriorytetow[5] += 50;
                    }
                    if ((map[positionX, positionY + 2] == 20 && gameManager.players[1].Orientation == Direction.RIGHT)
                    || (map[positionX, positionY + 2] == 10 && gameManager.players[0].Orientation == Direction.RIGHT))
                    {
                        listaPriorytetow[5] += 50;
                    }
                    if ((map[positionX + 1, positionY + 1] == 20 && gameManager.players[1].Orientation == Direction.RIGHT)
                    || (map[positionX + 1, positionY + 1] == 10 && gameManager.players[0].Orientation == Direction.RIGHT))
                    {
                        listaPriorytetow[5] += 50;
                    }
                }
                break;
            case Direction.RIGHT:
                if (map[positionX + 1, positionY] == 0)
                {
                    if ((map[positionX + 1, positionY + 1] == 20 && gameManager.players[1].Orientation == Direction.RIGHT)
                    || (map[positionX + 1, positionY + 1] == 10 && gameManager.players[0].Orientation == Direction.RIGHT))
                    {
                        listaPriorytetow[5] += 50;
                    }
                    if ((map[positionX + 2, positionY] == 20 && gameManager.players[1].Orientation == Direction.RIGHT)
                    || (map[positionX + 2, positionY] == 10 && gameManager.players[0].Orientation == Direction.RIGHT))
                    {
                        listaPriorytetow[5] += 50;
                    }
                    if ((map[positionX + 1, positionY - 1] == 20 && gameManager.players[1].Orientation == Direction.RIGHT)
                    || (map[positionX + 1, positionY - 1] == 10 && gameManager.players[0].Orientation == Direction.RIGHT))
                    {
                        listaPriorytetow[5] += 50;
                    }
                }
                break;
            case Direction.DOWN:
                if (map[positionX, positionY - 1] == 0)
                {
                    if ((map[positionX + 1, positionY - 1] == 20 && gameManager.players[1].Orientation == Direction.RIGHT)
                    || (map[positionX + 1, positionY - 1] == 10 && gameManager.players[0].Orientation == Direction.RIGHT))
                    {
                        listaPriorytetow[5] += 50;
                    }
                    if ((map[positionX, positionY - 2] == 20 && gameManager.players[1].Orientation == Direction.RIGHT)
                    || (map[positionX, positionY - 2] == 10 && gameManager.players[0].Orientation == Direction.RIGHT))
                    {
                        listaPriorytetow[5] += 50;
                    }
                    if ((map[positionX - 1, positionY - 1] == 20 && gameManager.players[1].Orientation == Direction.RIGHT)
                    || (map[positionX - 1, positionY - 1] == 10 && gameManager.players[0].Orientation == Direction.RIGHT))
                    {
                        listaPriorytetow[5] += 50;
                    }
                }
                break;
            case Direction.LEFT:
                if (map[positionX - 1, positionY] == 0)
                {
                    if ((map[positionX - 1, positionY + 1] == 20 && gameManager.players[1].Orientation == Direction.RIGHT)
                    || (map[positionX - 1, positionY + 1] == 10 && gameManager.players[0].Orientation == Direction.RIGHT))
                    {
                        listaPriorytetow[5] += 50;
                    }
                    if ((map[positionX - 2, positionY] == 20 && gameManager.players[1].Orientation == Direction.RIGHT)
                    || (map[positionX - 2, positionY] == 10 && gameManager.players[0].Orientation == Direction.RIGHT))
                    {
                        listaPriorytetow[5] += 50;
                    }
                    if ((map[positionX - 1, positionY - 1] == 20 && gameManager.players[1].Orientation == Direction.RIGHT)
                    || (map[positionX - 1, positionY - 1] == 10 && gameManager.players[0].Orientation == Direction.RIGHT))
                    {
                        listaPriorytetow[5] += 50;
                    }
                }
                break;
        }
        return listaPriorytetow;
    }

    int Move(int index)
    {
        map = gameManager.GetMap();
        Action ruch = Action.Wait;
        List<Action> listaKrokow2 = new List<Action>();
        int iloscRuchow = 100;
        int positionX = gameManager.players[2].x;
        int positionY = gameManager.players[2].y;
        switch (index)
        {

            //podchodzimy do gracza
            case 0:
                iloscRuchow = 100;
                //gracz 1
                if (gameManager.players[0].health > 0)
                {
                    if (map[gameManager.players[0].x - 1, gameManager.players[0].y] == 0)
                    {
                        listaKrokow2 = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y),
                                                                    gameManager.players[2].Orientation,
                                                                    new Vector2(gameManager.players[0].x - 1, gameManager.players[0].y),
                                                                    gameManager.players[2].Orientation);
                        if (listaKrokow2 != null && iloscRuchow > listaKrokow2.Count)
                        {
                            iloscRuchow = listaKrokow2.Count;
                            ruch = listaKrokow2[0];
                        }
                    }
                    if (map[gameManager.players[0].x, gameManager.players[0].y + 1] == 0)
                    {
                        listaKrokow2 = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y),
                                                                    gameManager.players[2].Orientation,
                                                                    new Vector2(gameManager.players[0].x, gameManager.players[0].y + 1),
                                                                    gameManager.players[2].Orientation);
                        if (listaKrokow2 != null && iloscRuchow > listaKrokow2.Count)
                        {
                            iloscRuchow = listaKrokow2.Count;
                            ruch = listaKrokow2[0];
                        }
                    }
                    if (map[gameManager.players[0].x + 1, gameManager.players[0].y] == 0)
                    {
                        listaKrokow2 = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y),
                                                                    gameManager.players[2].Orientation,
                                                                    new Vector2(gameManager.players[0].x + 1, gameManager.players[0].y),
                                                                    gameManager.players[2].Orientation);
                        if (listaKrokow2 != null && iloscRuchow > listaKrokow2.Count)
                        {
                            iloscRuchow = listaKrokow2.Count;
                            ruch = listaKrokow2[0];
                        }
                    }
                    if (map[gameManager.players[0].x, gameManager.players[0].y - 1] == 0)
                    {
                        listaKrokow2 = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y),
                                                                    gameManager.players[2].Orientation,
                                                                    new Vector2(gameManager.players[0].x, gameManager.players[0].y - 1),
                                                                    gameManager.players[2].Orientation);
                        if (listaKrokow2 != null && iloscRuchow > listaKrokow2.Count)
                        {
                            iloscRuchow = listaKrokow2.Count;
                            ruch = listaKrokow2[0];
                        }
                    }
                }
                //Gracz 2
                if (gameManager.players[1].health > 0)
                {
                    if (map[gameManager.players[1].x - 1, gameManager.players[1].y] == 0)
                    {
                        listaKrokow2 = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y),
                                                                    gameManager.players[2].Orientation,
                                                                    new Vector2(gameManager.players[1].x - 1, gameManager.players[1].y),
                                                                    gameManager.players[2].Orientation);
                        if (listaKrokow2 != null && iloscRuchow > listaKrokow2.Count)
                        {
                            iloscRuchow = listaKrokow2.Count;
                            ruch = listaKrokow2[0];
                        }
                    }
                    if (map[gameManager.players[1].x, gameManager.players[1].y + 1] == 0)
                    {
                        listaKrokow2 = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y),
                                                                    gameManager.players[2].Orientation,
                                                                    new Vector2(gameManager.players[1].x, gameManager.players[1].y + 1),
                                                                    gameManager.players[2].Orientation);
                        if (listaKrokow2 != null && iloscRuchow > listaKrokow2.Count)
                        {
                            iloscRuchow = listaKrokow2.Count;
                            ruch = listaKrokow2[0];
                        }
                    }
                    if (map[gameManager.players[1].x + 1, gameManager.players[1].y] == 0)
                    {
                        listaKrokow2 = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y),
                                                                    gameManager.players[2].Orientation,
                                                                    new Vector2(gameManager.players[1].x + 1, gameManager.players[1].y),
                                                                    gameManager.players[2].Orientation);
                        if (listaKrokow2 != null && iloscRuchow > listaKrokow2.Count)
                        {
                            iloscRuchow = listaKrokow2.Count;
                            ruch = listaKrokow2[0];
                        }
                    }
                    if (map[gameManager.players[1].x, gameManager.players[1].y - 1] == 0)
                    {
                        listaKrokow2 = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y),
                                                                    gameManager.players[2].Orientation,
                                                                    new Vector2(gameManager.players[1].x, gameManager.players[1].y - 1),
                                                                    gameManager.players[2].Orientation);
                        if (listaKrokow2 != null && iloscRuchow > listaKrokow2.Count)
                        {
                            iloscRuchow = listaKrokow2.Count;
                            ruch = listaKrokow2[0];
                        }
                    }
                }
                //jesli nie mamy podejscia to czekamy
                if (iloscRuchow == 100)
                {
                    ruch = Action.Wait;
                }
                break;

            case 1:
            case 2:
                iloscRuchow = 100;

                if (map[gameManager.players[2].x - 1, gameManager.players[2].y] == 0)
                {
                    listaKrokow2 = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y),
                                                                gameManager.players[2].Orientation,
                                                                new Vector2(gameManager.players[2].x - 1, gameManager.players[2].y),
                                                                Direction.LEFT);
                    if (listaKrokow2 != null && iloscRuchow > listaKrokow2.Count)
                    {
                        iloscRuchow = listaKrokow2.Count;
                        ruch = listaKrokow2[0];
                    }
                }
                if (map[gameManager.players[2].x, gameManager.players[2].y + 1] == 0)
                {
                    listaKrokow2 = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y),
                                                                gameManager.players[2].Orientation,
                                                                new Vector2(gameManager.players[2].x, gameManager.players[2].y + 1),
                                                                Direction.UP);
                    if (listaKrokow2 != null && iloscRuchow > listaKrokow2.Count)
                    {
                        iloscRuchow = listaKrokow2.Count;
                        ruch = listaKrokow2[0];
                    }
                }
                if (map[gameManager.players[2].x + 1, gameManager.players[2].y] == 0)
                {
                    listaKrokow2 = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y),
                                                                gameManager.players[2].Orientation,
                                                                new Vector2(gameManager.players[2].x + 1, gameManager.players[2].y),
                                                                Direction.RIGHT);
                    if (listaKrokow2 != null && iloscRuchow > listaKrokow2.Count)
                    {
                        iloscRuchow = listaKrokow2.Count;
                        ruch = listaKrokow2[0];
                    }
                }
                if (map[gameManager.players[2].x, gameManager.players[2].y - 1] == 0)
                {
                    listaKrokow2 = gameManager.aStar(new Vector2(gameManager.players[2].x, gameManager.players[2].y),
                                                                gameManager.players[2].Orientation,
                                                                new Vector2(gameManager.players[2].x, gameManager.players[2].y - 1),
                                                                Direction.DOWN);
                    if (listaKrokow2 != null && iloscRuchow > listaKrokow2.Count)
                    {
                        iloscRuchow = listaKrokow2.Count;
                        ruch = listaKrokow2[0];
                    }
                }
                //jesli nie mozemy uciec od bomby
                if (iloscRuchow == 100)
                {
                    ruch = Action.Wait;
                }
                break;

            case 3:
                iloscRuchow = 100;
                List<Vector2> listaBomb = gameManager.GetBombList();
                ruch = Action.Wait;
                break;

            case 4:
                iloscRuchow = 100;
                //lewo
                if (map[positionX - 1, positionY] == 0)
                {
                    if ((map[positionX - 1, positionY - 1] == 10 || map[positionX - 1, positionY - 1] == 20) ||
                        (map[positionX - 2, positionY] == 10 || map[positionX - 2, positionY] == 20) ||
                        (map[positionX - 1, positionY + 1] == 10 || map[positionX - 1, positionY + 1] == 20))
                    {
                        if (gameManager.players[2].Orientation == Direction.LEFT)
                        {
                            Debug.Log("Bombka lewo");
                            gameManager.PlaceBomb(playerIndex);
                            ruch = Action.Wait;
                        }
                        else
                        {
                            if (gameManager.players[2].Orientation == Direction.UP)
                            {
                                if (iloscRuchow > 1)
                                {
                                    iloscRuchow = 1;
                                    ruch = Action.RotateCounterClockwise;
                                }
                            }
                            else if (gameManager.players[2].Orientation == Direction.RIGHT)
                            {
                                if (iloscRuchow > 2)
                                {
                                    iloscRuchow = 2;
                                    ruch = Action.RotateCounterClockwise;
                                }
                            }
                            else if (gameManager.players[2].Orientation == Direction.DOWN)
                            {
                                if (iloscRuchow > 1)
                                {
                                    iloscRuchow = 1;
                                    ruch = Action.RotateClockwise;
                                }
                            }
                        }

                    }
                }
                //gora
                if (map[positionX, positionY + 1] == 0)
                {
                    if ((map[positionX - 1, positionY + 1] == 10 || map[positionX - 1, positionY + 1] == 20) ||
                        (map[positionX, positionY + 2] == 10 || map[positionX, positionY + 2] == 20) ||
                        (map[positionX + 1, positionY + 1] == 10 || map[positionX + 1, positionY + 1] == 20))
                    {
                        if (gameManager.players[2].Orientation == Direction.UP)
                        {
                            Debug.Log("Bombka gora");
                            gameManager.PlaceBomb(playerIndex);
                            ruch = Action.Wait;
                        }
                        else
                        {
                            if (gameManager.players[2].Orientation == Direction.RIGHT)
                            {
                                if (iloscRuchow > 1)
                                {
                                    iloscRuchow = 1;
                                    ruch = Action.RotateCounterClockwise;
                                }
                            }
                            else if (gameManager.players[2].Orientation == Direction.DOWN)
                            {
                                if (iloscRuchow > 2)
                                {
                                    iloscRuchow = 2;
                                    ruch = Action.RotateCounterClockwise;
                                }
                            }
                            else if (gameManager.players[2].Orientation == Direction.LEFT)
                            {
                                if (iloscRuchow > 1)
                                {
                                    iloscRuchow = 1;
                                    ruch = Action.RotateClockwise;
                                }
                            }
                        }

                    }
                }
                //prawo
                if (map[positionX + 1, positionY] == 0)
                {
                    if ((map[positionX + 1, positionY + 1] == 10 || map[positionX + 1, positionY + 1] == 20) ||
                        (map[positionX + 2, positionY] == 10 || map[positionX + 2, positionY] == 20) ||
                        (map[positionX + 1, positionY - 1] == 10 || map[positionX + 1, positionY - 1] == 20))
                    {
                        if (gameManager.players[2].Orientation == Direction.RIGHT)
                        {
                            Debug.Log("Bombka prawo");
                            gameManager.PlaceBomb(playerIndex);
                            ruch = Action.Wait;
                        }
                        else
                        {
                            if (gameManager.players[2].Orientation == Direction.DOWN)
                            {
                                if (iloscRuchow > 1)
                                {
                                    iloscRuchow = 1;
                                    ruch = Action.RotateCounterClockwise;
                                }
                            }
                            else if (gameManager.players[2].Orientation == Direction.LEFT)
                            {
                                if (iloscRuchow > 2)
                                {
                                    iloscRuchow = 2;
                                    ruch = Action.RotateCounterClockwise;
                                }
                            }
                            else if (gameManager.players[2].Orientation == Direction.UP)
                            {
                                if (iloscRuchow > 1)
                                {
                                    iloscRuchow = 1;
                                    ruch = Action.RotateClockwise;
                                }
                            }
                        }

                    }
                }
                //dol
                if (map[positionX, positionY - 1] == 0)
                {
                    if ((map[positionX + 1, positionY - 1] == 10 || map[positionX + 1, positionY - 1] == 20) ||
                        (map[positionX, positionY - 2] == 10 || map[positionX, positionY - 2] == 20) ||
                        (map[positionX - 1, positionY - 1] == 10 || map[positionX - 1, positionY - 1] == 20))
                    {
                        if (gameManager.players[2].Orientation == Direction.DOWN)
                        {
                            Debug.Log("Bombka dol");
                            gameManager.PlaceBomb(playerIndex);
                            ruch = Action.Wait;
                        }
                        else
                        {
                            if (gameManager.players[2].Orientation == Direction.LEFT)
                            {
                                if (iloscRuchow > 1)
                                {
                                    iloscRuchow = 1;
                                    ruch = Action.RotateCounterClockwise;
                                }
                            }
                            else if (gameManager.players[2].Orientation == Direction.UP)
                            {
                                if (iloscRuchow > 2)
                                {
                                    iloscRuchow = 2;
                                    ruch = Action.RotateCounterClockwise;
                                }
                            }
                            else if (gameManager.players[2].Orientation == Direction.RIGHT)
                            {
                                if (iloscRuchow > 1)
                                {
                                    iloscRuchow = 1;
                                    ruch = Action.RotateClockwise;
                                }
                            }
                        }

                    }
                }
                break;
            case 5:
                Debug.Log("bomba - direction");
                gameManager.PlaceBomb(playerIndex);
                ruch = Action.Wait;
                break;
        }

        switch (ruch)
        {
            case Action.Wait:
                Debug.Log("Czekam");
                return -1;
                break;

            case Action.MoveForward:
                gameManager.MoveForward(playerIndex);
                Debug.Log("MoveForward");
                return -1;
                break;

            case Action.RotateClockwise:
                gameManager.RotateClockwise(playerIndex);
                Debug.Log("Obrot prawo");
                return -1;
                break;

            case Action.RotateCounterClockwise:
                gameManager.RotateCounterClockwise(playerIndex);
                Debug.Log("Obrot lewo");
                return -1;
                break;
        }
        return -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > turnTime)
        {

            int[] listaPriorytetow = MinMax();

            int[] listaPriorytetowIndex = new int[6];
            for (int i = 0; i < listaPriorytetow.Length; i++)
            {
                listaPriorytetowIndex[i] = listaPriorytetow[i];
            }

            Array.Sort<int>(listaPriorytetowIndex);
            foreach (int element in listaPriorytetowIndex)
            {
                Debug.Log(element);
            }


            int tmp = 1;
            int licznik = 5;
            while (tmp > 0)
            {
                int wartosc = listaPriorytetowIndex[licznik];
                for (int i = 0; i < listaPriorytetow.Length; i++)
                {
                    if (listaPriorytetow[i] == wartosc)
                    {
                        tmp = Move(i);
                        Debug.Log("i = " + i.ToString());
                        Debug.Log("wartosc = " + wartosc.ToString());
                        break;
                    }

                }
                licznik--;
            }



            timer = 0.0f;
        }
        timer += Time.deltaTime;
    }
}

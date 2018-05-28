using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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
        Direction orientation = gameManager.players[2].orientation;

        int[] listaPriorytetow = new int [5] {0, 0, 0, 0, 0};


        //obok wood, sciana, woda, obok
        /* PO LEWEJ */
        if (map[positionX-1, positionY] == 8 || map[positionX-1, positionY] == 6 ||) {
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
	
	// Update is called once per frame
	void Update () {
        if (timer > turnTime) {
            
            int[] listaPriorytetow = MinMax();
    

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
            


            timer = 0.0f;
        }
        timer += Time.deltaTime;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MinMaxAi : MonoBehaviour {

    GameManager gameManager;
    public int playerIndex = 10;

    //Timers
    float timer = 0.0f;
    float turnTime;

	// Use this for initialization
	void Start () {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        turnTime = gameManager.GetTurnTime();
		
	}
	
	// Update is called once per frame
	void Update () {
        if (timer > turnTime) {
            // Możliwości: 
            // idź - koszt 1
            // obrót w lewo - koszt 1
            // obrót w prawo - koszt 1
            // czekaj - koszt 0
            // postaw bombę - koszt 1
            // pola:
            // bomba i kratka w lewo, prawo, gora, dół, d = -1000
            // puste
            // przeciwnicy
            // woda
            


            timer = 0.0f;
        }
        timer += Time.deltaTime;
	}
}

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
        // turnTime = gameManager.GetTurnTime();
		
	}
	
	// Update is called once per frame
	void Update () {
        if (timer > turnTime) {
            //Tutaj pojedyncza runda sie rozgrywa
            
            //Mozesz dac tutaj np, czyli dopoki gra sie nie skonczy
            // while(gameManager.GameFinished() == -1){
                   
            // }
            


            timer = 0.0f;
        }
        timer += Time.deltaTime;
	}
}

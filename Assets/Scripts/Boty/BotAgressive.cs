using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotAgressive : MonoBehaviour {

	GameManager gameManager;
	public int playerIndex = 10;
	
	//Timers
	float timer = 0.0f;
	float turnTime;
	
	void Start () {
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		turnTime = gameManager.GetTurnTime();
	}
	
	
	void Update () {
		if(timer > turnTime){
			//Make Move
			//1.Check for closest enemy
			float distance = 9999;
			float index = 0;
			for(int i = 10; i <= 30; i+=10)
			{
				if(i != playerIndex){
					float temp = Vector2.Distance(new Vector2(gameManager.players[(i / 10) - 1].x,gameManager.players[(i / 10) - 1].y)
												   ,new Vector2(gameManager.players[(playerIndex / 10) - 1].x,gameManager.players[(playerIndex / 10) - 1].y));
					if(distance < temp){
						distance = temp;
						index = i;						   	
					}
				}
			}
			//2.Check if u are close enough
			bool isClose;
			if(distance <= 2){
				isClose = true;
			} else{
				isClose = false;
			}


			timer = 0.0f;
		}

	timer += Time.deltaTime;
	}
}

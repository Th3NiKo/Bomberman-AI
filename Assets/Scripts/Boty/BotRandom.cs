using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

Bot check if can move or place bomb if can do it. If cant rotate random direction.

 */

public class BotRandom : MonoBehaviour {

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
			//Make random move
			int randomValue = Random.Range(1,6);
			if(randomValue < 5){
				if(gameManager.CanMoveForward(playerIndex)){
					gameManager.MoveForward(playerIndex);
				} else {
					randomValue = Random.Range(1,2);
					if(randomValue == 1){
						gameManager.RotateClockwise(playerIndex);
					} else {
						gameManager.RotateCounterClockwise(playerIndex);
					}
				}
			} else {
				if(gameManager.CanMoveForward(playerIndex)){
					gameManager.PlaceBomb(playerIndex);
				} else {
					randomValue = Random.Range(1,2);
					if(randomValue == 1){
						gameManager.RotateClockwise(playerIndex);
					} else {
						gameManager.RotateCounterClockwise(playerIndex);
					}
				}
			}
			timer = 0.0f;
		}
		
		timer += Time.deltaTime;
		}
		
}


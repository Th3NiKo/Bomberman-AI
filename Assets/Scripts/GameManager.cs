/* DICTIONARY 

Players
10 - Player 1 on map
20 - Player 2 on map
30 - Player 3 on map
(40 - Player 4 on map)* Now we got only 3 players but maybe later...

Walls
9 - Unbreakable wall
8 - Breakable wall

Terrain
0 - Empty space
(-1) - Not assigned

Items
4 - Bomb just placed
3 - Bomb waits
2 - Bomb waits
1 - Bomb blows

Boosts (not yet)





/////////////////////////////
oneBomb = 5 Bomb created but u need to move to show it
oneBomb = 4 Bomb shows
oneBomb = 3 Bomb ticks
oneBomb = 2 Bomb ticks
oneBomb = 1 Bomb blows
oneBomb = 0 No Bomb for player
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	//Prefabs representing array
	public GameObject emptyObject;
	public GameObject unbreakableObject;
	public GameObject breakableObject;
	public GameObject bombObject;
	public GameObject Player1;
	public GameObject Player2;
	public GameObject Player3;

	public GameObject explosionObject;
	public Material dmg;



	//Variables
	public int rowsCount;
	public int columnsCount;

	public int howMany = 15;
	private int[,] map;
	private float turnTimer = 0.0f;
	private float turnTime = 0.5f;

	//Guards
	private bool oneMoved = false;
	private int oneBomb = 0;
	private bool twoMoved = false;
	private int twoBomb = 0;
	private bool threeMoved = false;
	private int threeBomb = 0;

	//Lifes
	private int oneHealth = 3;
	private int twoHealth = 3;
	private int threeHealth = 3;

	void Start () {
		map = new int [rowsCount, columnsCount];
		createMap(howMany);
		renderMap();
	}
	
	
	void Update () {
		turnTimer += Time.deltaTime;
		//Keyboard input for tests
		if(Input.GetKey(KeyCode.UpArrow)){
			MoveUp(10);
		} else if(Input.GetKey(KeyCode.DownArrow)){
			MoveDown(10);
		} else if(Input.GetKey(KeyCode.LeftArrow)){
			MoveLeft(10);
		} else if(Input.GetKey(KeyCode.RightArrow)){
			MoveRight(10);
		} else if(Input.GetKey(KeyCode.Space)){
			PlaceBomb(10);
		}
		if(turnTimer >= turnTime){
			//If time passes everyone can move again
			removeAll();
			renderMap();
			turnTimer = 0.0f;
			oneMoved = false;
			twoMoved = false;
			threeMoved = false;
		}
	}


	void createMap(int howManyBreakable){
		//Set camera position so we will see everything
		//Doing some triangles calculations to get high of camera (Pitagoras etc)
		float basis = Mathf.Sqrt(Mathf.Pow(((rowsCount - 1) / 2),2) + Mathf.Pow(((rowsCount - 1) / 2),2));

		Camera.main.transform.position = new Vector3((rowsCount - 1) / 2,(basis * Mathf.Sqrt(3)) ,(columnsCount - 1) / 2);

		//We start with iniliazing with -1 all
		for(int i = 0; i < rowsCount; i++){
			for(int j = 0; j < columnsCount; j++){
				map[i,j] = -1;
			}
		}

		//Now we make edges of map and "every second" unbreakable (9)
		for(int i = 0; i < rowsCount; i++){
			for(int j = 0; j < columnsCount; j++){ //Edges
				if(i == 0 || j == 0 || i == (rowsCount - 1) || j == (columnsCount - 1)){
					map[i,j] = 9;
				} else if((i % 2 == 0) && (j % 2 == 0)){ //If numbers are both even we place unbreakable (9)
					map[i,j] = 9;
				}
			}
		}

		//We set players to their starting positions (10,20,30,40)
		map[1,1] = 10;
		map[1,columnsCount - 2] = 20;
		map[rowsCount - 2, 1] = 30;
		//and set near blocks empty (0) so they can leave without blowing urself
		map[1,2] = 0; map[2,1] = 0;
		map[1,columnsCount - 3] = 0; map[2, columnsCount - 2] = 0;
		map[rowsCount - 3, 1] = 0; map[rowsCount - 2, 2] = 0;

		//Now we will choose our breakable blocks randomly from these which are avaliable (-1)
		//First search for avaliable and add them to list
		List<Vector2> avabliableList = new List<Vector2>();
		for(int i = 0; i < rowsCount; i++){
			for(int j = 0; j < columnsCount; j++){
				if(map[i,j] == -1){
					Vector2 temp = new Vector2(i,j);
					avabliableList.Add(temp);
				}
			}
		}

		//Now we choose avaliable places randomly from our list and place there breakable (8)
		for(int i = 0; i < howManyBreakable; i++){
			if(avabliableList.Count > 0){ //If we got space to place
				int whichPlace = Random.Range(0, avabliableList.Count); //Choose random space from list
				map[(int)avabliableList[whichPlace].x,(int)avabliableList[whichPlace].y] = 8; //Place there
				avabliableList.RemoveAt(whichPlace); //Remove from empty spaces
			} else { //If we dont have avaliable place just break
				break;
			}
		}

		//If there are any other avaliable places change them to empty
		for(int i = 0;i < avabliableList.Count; i++){
			map[(int)avabliableList[i].x,(int)avabliableList[i].y] = 0;
		}
		
	}

	void renderMap(){
		
		//We need to start from some points of the map and draw every element
		//Debug.Log(oneBomb);
		float startX = 0.0f;
		float startY = 0.0f;
		for(int i = 0; i < rowsCount; i++){
			startY = 0.0f;
			for(int j = 0; j < columnsCount; j++){
				switch(map[i,j]){
					case 0:
						Instantiate(emptyObject,new Vector3(startX, 1f, startY), Quaternion.identity);
					break;
					case 8:
						Instantiate(breakableObject,new Vector3(startX, 1f, startY), Quaternion.identity);
					break;
					case 9:
						Instantiate(unbreakableObject,new Vector3(startX, 1f, startY), Quaternion.identity);
					break;
					case 10:
						Instantiate(Player1,new Vector3(startX, 1f, startY), Quaternion.identity);
						Instantiate(emptyObject,new Vector3(startX, 1f, startY), Quaternion.identity);
					break;
					case 20:
						Instantiate(Player2,new Vector3(startX, 1f, startY), Quaternion.identity);						
						Instantiate(emptyObject,new Vector3(startX, 1f, startY), Quaternion.identity);
					break;
					case 30:
						Instantiate(Player3,new Vector3(startX, 1f, startY), Quaternion.identity);
						Instantiate(emptyObject,new Vector3(startX, 1f, startY), Quaternion.identity);
					break;
					case 4:
						GameObject tempBomb;
						tempBomb = Instantiate(bombObject,new Vector3(startX, 1f, startY), Quaternion.identity);
						Instantiate(emptyObject,new Vector3(startX, 1f, startY), Quaternion.identity);
						tempBomb.transform.localScale = new Vector3(tempBomb.transform.localScale.x + 0.0f, tempBomb.transform.localScale.y + 0.0f, tempBomb.transform.localScale.z + 0.0f);
						map[i,j] = 3;
					break;
					case 3:
						tempBomb = Instantiate(bombObject,new Vector3(startX, 1f, startY), Quaternion.identity);
						Instantiate(emptyObject,new Vector3(startX, 1f, startY), Quaternion.identity);
						tempBomb.transform.localScale = new Vector3(tempBomb.transform.localScale.x + 0.1f, tempBomb.transform.localScale.y + 0.1f, tempBomb.transform.localScale.z + 0.1f);

						map[i,j] = 2;
					break;
					case 2:
						tempBomb = Instantiate(bombObject,new Vector3(startX, 1f, startY), Quaternion.identity);
						Instantiate(emptyObject,new Vector3(startX, 1f, startY), Quaternion.identity);
						tempBomb.transform.localScale = new Vector3(tempBomb.transform.localScale.x + 0.2f, tempBomb.transform.localScale.y + 0.2f, tempBomb.transform.localScale.z + 0.2f);

						map[i,j] = 1;
					break;
					case 1:
						//Blowing terrain
						
						if(map[i-1,j] == 0 || map[i-1,j] == 8){
							Instantiate(explosionObject,new Vector3(startX - 1f, 1f, startY), Quaternion.identity);
							map[i-1,j] = 0;
						} if (map[i+1,j] == 0 || map[i+1,j] == 8){
							Instantiate(explosionObject,new Vector3(startX + 1f, 1f, startY), Quaternion.identity);
							map[i+1,j] = 0;
						} if (map[i,j-1] == 0 || map[i,j-1] == 8){
							Instantiate(explosionObject,new Vector3(startX, 1f, startY -1f), Quaternion.identity);
							map[i,j-1] = 0;
						} if (map[i,j+1] == 0 || map[i,j+1] == 8){
							Instantiate(explosionObject,new Vector3(startX, 1f, startY +1f), Quaternion.identity);
							map[i,j+1] = 0;
						} 
						Instantiate(explosionObject,new Vector3(startX, 1f, startY), Quaternion.identity);
						Instantiate(emptyObject,new Vector3(startX, 1f, startY), Quaternion.identity);

						map[i,j] = 0;



						//Now blowing players
						if(map[i-1,j] == 10 || map[i-1,j] == 20 ||  map[i-1,j] == 30){
							//Color red for one second
							StartCoroutine(changeColor(map[i-1,j]));
							//Take health down
							takeHealthDown(map[i-1,j]);
						}if(map[i+1,j] == 10 || map[i+1,j] == 20 ||  map[i+1,j] == 30){
							//Color red for one second
							StartCoroutine(changeColor(map[i+1,j]));
							//Take health down
							takeHealthDown(map[i+1,j]);
						}if(map[i,j-1] == 10 || map[i,j-1] == 20 ||  map[i,j-1] == 30){
							//Color red for one second
							StartCoroutine(changeColor(map[i,j-1]));
							//Take health down
							takeHealthDown(map[i,j-1]);
						} if(map[i,j+1] == 10 || map[i,j+1] == 20 ||  map[i,j+1] == 30){
							//Color red for one second
							StartCoroutine(changeColor(map[i,j+1]));
							//Take health down
							takeHealthDown(map[i,j+1]);
						}
					break;

						
				}
				
				startY += 1f;
			}
			startX += 1f;
		}
		bombsTickDown(); //All bombs counter down (guards);

	}

	void removeAll(){
		 GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>() ;
 		 foreach(GameObject go in allObjects)
    		if (go.layer != 8){
				Destroy(go.gameObject);
			}

	}

	//Functions to use from other scripts are here
	public int[,] getMap(){
		return map;
	}

	public void MoveUp(int playerIndex){
		for(int i = 0; i < rowsCount; i++){
			for(int j = 0; j < columnsCount; j++){
				if((map[i,j] == playerIndex) && (map[i,j + 1] == 0)){
					if(canMove(playerIndex)){
						map[i,j + 1] = playerIndex;
						playerMoved(playerIndex);
						if(shouldCreateBomb(playerIndex)){ //Bomb is on state 5 so we need to create it 
							map[i,j] = 4;
							bombCreated(playerIndex);

						} else {
							map[i,j] = 0;
						}
						break;
					}
				}
			}
		}
		
	}

	public void MoveDown(int playerIndex){
		for(int i = 0; i < rowsCount; i++){
			for(int j = 0; j < columnsCount; j++){
				if((map[i,j] == playerIndex) && (map[i,j - 1] == 0)){
					if(canMove(playerIndex)){
						map[i,j - 1] = playerIndex;
						if(shouldCreateBomb(playerIndex)){ //Bomb is on state 5 so we need to create it 
							map[i,j] = 4;
							bombCreated(playerIndex);
						} else {
							map[i,j] = 0;
						}
						playerMoved(playerIndex);
						break;
					}
				}
			}
		}
	}

	public void MoveLeft(int playerIndex){
		for(int i = 1; i < rowsCount; i++){
			for(int j = 1; j < columnsCount; j++){
				if((map[i,j] == playerIndex) && (map[i - 1,j] == 0)){
					if(canMove(playerIndex)){
						map[i - 1,j] = playerIndex;
						if(shouldCreateBomb(playerIndex)){ //Bomb is on state 5 so we need to create it 
							map[i,j] = 4;
							bombCreated(playerIndex);
						} else {
							map[i,j] = 0;
						}
						playerMoved(playerIndex);	
						break;				
					}
				}
			}
		}
	}

	public void MoveRight(int playerIndex){
		for(int i = 0; i < rowsCount; i++){
			for(int j = 0; j < columnsCount; j++){
				if((map[i,j] == playerIndex) && (map[i + 1,j] == 0)){
					if(canMove(playerIndex)){
						map[i + 1,j] = playerIndex;
						playerMoved(playerIndex);
						if(shouldCreateBomb(playerIndex)){ //Bomb is on state 5 so we need to create it 
							map[i,j] = 4;
							bombCreated(playerIndex);
						} else {
							map[i,j] = 0;
						}						
					}
				}
			}
		}
	}

	public void PlaceBomb(int playerIndex){
		if(!isBombPlaced(playerIndex)){ //If player doesnt have bomb on a map
			if(playerIndex == 10){
				oneBomb = 5;
			} else if(playerIndex == 20){
				twoBomb = 5;
			} else if(playerIndex == 30){
				threeBomb = 5;
			}
		}
	}


	//Guards functions
	private bool canMove(int playerIndex){ //Can player move?
		if(playerIndex == 10){
			if(oneMoved == false){
				return true;
			}
		} else if(playerIndex == 20){
			if(twoMoved == false){
				return true;
			}	
		} else if(playerIndex == 30){
			if(threeMoved == false){
				return true;
			}	
		}
		return false;
	}

	private void playerMoved(int playerIndex){ //Player has moved
		if(playerIndex == 10){
			oneMoved = true;
		} else if(playerIndex == 20){
			twoMoved = true;
		} else if(playerIndex == 30){
			threeMoved = true;
		}
	}

	private bool isBombPlaced(int playerIndex){ //Is player bomb placed already?
		if(playerIndex == 10){
			if(oneBomb == 0){
				return false;
			} else {
				return true;
			}
		} else if(playerIndex == 20){
			if(twoBomb == 0){
				return false;
			} else {
				return true;
			}
		} else if(playerIndex == 30){
			if(threeBomb == 0){
				return false;
			} else {
				return true;
			}
		}
		return false;
	}

	private bool shouldCreateBomb(int playerIndex){ //Do i need to create bomb after move?
		if(playerIndex == 10){
			if(oneBomb != 5){
				return false;
			} else {
				return true;
			}
		} else if(playerIndex == 20){
			if(twoBomb != 5){
				return false;
			} else {
				return true;
			}
		} else if(playerIndex == 30){
			if(threeBomb != 5){
				return false;
			} else {
				return true;
			}
		}
		return false;
	}

	private void bombsTickDown(){
		if(oneBomb > 0 && oneBomb < 5){
			oneBomb--;
		}
		if(twoBomb > 0 && twoBomb < 5){
			twoBomb--;
		}
		if(threeBomb > 0 && threeBomb < 5){
			threeBomb--;
		}
	}

	private void bombCreated(int playerIndex){
		if(playerIndex == 10){
			oneBomb = 4;
		} else if(playerIndex == 20){
			twoBomb = 4;
		} else if(playerIndex == 30){
			threeBomb = 4;
		}
	}

	private void takeHealthDown(int playerIndex){
		if(playerIndex == 10){
			oneHealth--;
		} else if(playerIndex == 20){
			twoHealth--;
		} else if(playerIndex == 30){
			threeHealth--;
		}
	}

	private IEnumerator changeColor(int playerIndex){
		GameObject temp;
		 if(playerIndex == 10){
			temp = Player1;
		 } else if(playerIndex == 20){
			temp = Player2;
		 } else if(playerIndex == 30){
			temp = Player3;
		 } else {
			 temp = Player1;
		 }
		 Material tempColor = temp.GetComponent<MeshRenderer>().sharedMaterial;
		 temp.GetComponent<MeshRenderer>().sharedMaterial = dmg;
		 yield return new WaitForSeconds(.1f);
		 temp.GetComponent<MeshRenderer>().sharedMaterial = tempColor;

	}
}

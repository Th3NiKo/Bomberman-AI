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
6 - Water
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



Funkcje publiczne (do użytku):

RotateClockwise(int playerIndex)
RotateCounterClockwise(int playerIndex)
MoveForward(int playerIndex)
PlaceBomb(int playerIndex)
CheckHp(int playerIndex) //Zwraca hp danego gracza
GetMap()
GameFinished() //Zwraca numer gracza ktory wygral jezeli gra sie zakonczyla. Jezeli gra nadal trwa zwraca -1
GetTurnTime() //Zwraca ile trwa runda
CanMoveForward(int playerIndex) //Zwraca czy mozemy sie ruszyc prosto

*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Node {
	public float g;
	public float h;
	public float f;

	public Node parent;
	public Vector2 position;
	public Direction orientation;

	public int waterPenalty;


	public Node(Node _parent, Vector2 _position, Direction _orient){
		g = 0.0f;
		h = 0.0f;
		f = 0.0f;
		parent = _parent;
		position = _position;
		orientation = _orient;
		waterPenalty = 0;
	}

	public Node(){

	}
}


public enum Direction {DOWN, LEFT, RIGHT, UP};

public enum Action {RotateClockwise, RotateCounterClockwise, MoveForward, Wait, PlaceBomb}

public class Player{
	public Direction Orientation;
	public int x;
	public int y;

	public int health;

	public int bombState;
	public Vector2 bombPosition;
	
	public int killedPlayer;
	public bool killedWall;
	
	public Action lastAction;

	public Player(Direction o, int _x, int _y){
		bombPosition = new Vector2(-1,-1);
		Orientation = o;
		x = _x;
		y = _y;
		bombState = 0;
		health = 3;
		lastAction = Action.Wait;
		killedPlayer = -1;
		killedWall = false;
	}

};


public class GameManager : MonoBehaviour {

	//Prefabs representing array
	
	public GameObject emptyObject;
	public GameObject waterObject;
	public GameObject unbreakableObject;
	public GameObject breakableObject;
	public GameObject bombObject;
	public GameObject Player1;
	public GameObject Player2;
	public GameObject Player3;
	public GameObject Marker;

	public GameObject explosionObject;
	public Material dmg;

	public Vector2 startPoint;
	public Vector2 endPoint;


	//Variables
	public int rowsCount;
	public int columnsCount;

	public int howMany = 15;
	private int[,] map;
	private float turnTimer = 0.0f;
	private float turnTime = 0.1f;

	//Guards
	private bool oneMoved = false;
	private int oneBomb = 0;
	private bool twoMoved = false;
	private int twoBomb = 0;
	private bool threeMoved = false;
	private int threeBomb = 0;

	private int gameWonBy = -1;

	//Lifes
	private int oneHealth = 3;
	private int twoHealth = 3;
	private int threeHealth = 3;
	List<Action> listaKrokow;

	public List<Player> players;
	bool once = true;
	int counterMain = 0;
	void Start () {
		players = new List<Player>();
		map = new int [rowsCount, columnsCount];
		createMap(howMany);
		renderMap();

		listaKrokow	= aStar(startPoint,players[0].Orientation, endPoint, Direction.DOWN);
		
		
	}
	



	void Update () {
		turnTimer += Time.deltaTime;
		//Keyboard input for tests
		
		if(Input.GetKey(KeyCode.LeftArrow)){
RotateCounterClockwise(30);
		} if(Input.GetKey(KeyCode.RightArrow)){			RotateClockwise(30);} 
		if(Input.GetKey(KeyCode.Space)){
			PlaceBomb(30);
		} if(Input.GetKey(KeyCode.UpArrow)){
			MoveForward(30);
		}
		

		
		if(turnTimer >= turnTime){
			//If time passes everyone can move again
			removeAll();
			renderMap();
			turnTimer = 0.0f;
			oneMoved = false;
			twoMoved = false;
			threeMoved = false;

			if(listaKrokow != null && counterMain < listaKrokow.Count){
				if(listaKrokow[counterMain] == Action.MoveForward){
					//MoveForward(10);
				} else if (listaKrokow[counterMain] == Action.RotateClockwise){
					//RotateClockwise(10);
				} else {
					//RotateCounterClockwise(10);
				}
			}
			counterMain++;
		}
		

		if(once == true){
			if(listaKrokow != null){
				for(int i = 0; i < listaKrokow.Count; i++){
					//Debug.log(listaKrokow[i].ToString());
					////Debug.log("Znajduje tam sie: " + map[(int)listaKrokow[i].x, (int)listaKrokow[i].y]);
					//Instantiate(Marker, new Vector3(listaKrokow[i].x, 1.1f, listaKrokow[i].y), Quaternion.identity);
				}
			}
			once = false;
		} 
	}


	public void createMap(int howManyBreakable){
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

		Player p1 = new Player(Direction.RIGHT, 1,1);
		Player p2 = new Player(Direction.DOWN, 1,columnsCount - 2);
		Player p3 = new Player(Direction.LEFT, rowsCount - 2,1);
		players.Add(p1); //10
		players.Add(p2); //20
		players.Add(p3); //30
		//and set near blocks empty (0) so they can leave without blowing urself
		map[1,2] = 0; map[2,1] = 0;
		map[1,columnsCount - 3] = 0; map[2, columnsCount - 2] = 0;
		map[rowsCount - 3, 1] = 0; map[rowsCount - 2, 2] = 0;
		map[rowsCount / 2, columnsCount / 2] = 6;
		map[rowsCount / 2 - 1, columnsCount / 2] = 6;
		map[rowsCount / 2 + 1, columnsCount / 2] = 6;
		map[rowsCount / 2, columnsCount / 2 - 1] = 6;
		map[rowsCount / 2, columnsCount / 2 + 1] = 6;
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
		////Debug.log(oneBomb);
		float startX = 0.0f;
		float startY = 0.0f;
		
		for(int i = 0; i < rowsCount; i++){
			startY = 0.0f;
			for(int j = 0; j < columnsCount; j++){


				switch(map[i,j]){
					case 0:
						Instantiate(emptyObject,new Vector3(startX, 1f, startY), Quaternion.identity);
					break;
					case 6:
						Instantiate(waterObject,new Vector3(startX, 1f, startY), Quaternion.identity);
					break;
					case 8:
						Instantiate(breakableObject,new Vector3(startX, 1f, startY), Quaternion.identity);
					break;
					case 9:
						Instantiate(unbreakableObject,new Vector3(startX, 1f, startY), Quaternion.identity);
					break;
					case 10:
						Instantiate(Player1,new Vector3(startX, 1f, startY), setRotation(10));
						Instantiate(emptyObject,new Vector3(startX, 1f, startY), Quaternion.identity);
					break;
					case 20:
						Instantiate(Player2,new Vector3(startX, 1f, startY), setRotation(20));						
						Instantiate(emptyObject,new Vector3(startX, 1f, startY), Quaternion.identity);
					break;
					case 30:
						Instantiate(Player3,new Vector3(startX, 1f, startY), setRotation(30));
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
						int bombOfPlayer = whosBombItIs(i,j);
						////Debug.log("PLayer numer" + bombOfPlayer + " bomb blowed");
						
						if(map[i-1,j] == 0 || map[i-1,j] == 8){
							if(map[i-1,j] == 8){
								if(bombOfPlayer >= 0)
								players[bombOfPlayer].killedWall = true;
							}
							Instantiate(explosionObject,new Vector3(startX - 1f, 1f, startY), Quaternion.identity);
							map[i-1,j] = 0;
						} if (map[i+1,j] == 0 || map[i+1,j] == 8){
							if(map[i+1,j] == 8){
								if(bombOfPlayer >= 0)
								players[bombOfPlayer].killedWall = true;
							}
							Instantiate(explosionObject,new Vector3(startX + 1f, 1f, startY), Quaternion.identity);
							map[i+1,j] = 0;
						} if (map[i,j-1] == 0 || map[i,j-1] == 8){
							if(map[i,j-1] == 8){
								if(bombOfPlayer >= 0)
								players[bombOfPlayer].killedWall = true;
							}
							Instantiate(explosionObject,new Vector3(startX, 1f, startY -1f), Quaternion.identity);
							map[i,j-1] = 0;
						} if (map[i,j+1] == 0 || map[i,j+1] == 8){
							if(map[i,j+1] == 8){
								if(bombOfPlayer >= 0)
								players[bombOfPlayer].killedWall = true;
							}
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
							if(CheckHp(map[i-1,j]) <= 0){
								if(bombOfPlayer >= 0)
								players[bombOfPlayer].killedPlayer = map[i-1,j];
							}
						}if(map[i+1,j] == 10 || map[i+1,j] == 20 ||  map[i+1,j] == 30){
							//Color red for one second
							StartCoroutine(changeColor(map[i+1,j]));
							//Take health down
							takeHealthDown(map[i+1,j]);
							if(CheckHp(map[i+1,j]) <= 0){
								if(bombOfPlayer >= 0)
								players[bombOfPlayer].killedPlayer = map[i+1,j];
							}
						}if(map[i,j-1] == 10 || map[i,j-1] == 20 ||  map[i,j-1] == 30){
							//Color red for one second
							StartCoroutine(changeColor(map[i,j-1]));
							//Take health down
							takeHealthDown(map[i,j-1]);
							if(CheckHp(map[i,j-1]) <= 0){
								if(bombOfPlayer >= 0)
								players[bombOfPlayer].killedPlayer = map[i,j-1];
							}
						} if(map[i,j+1] == 10 || map[i,j+1] == 20 ||  map[i,j+1] == 30){
							//Color red for one second
							StartCoroutine(changeColor(map[i,j+1]));
							//Take health down
							takeHealthDown(map[i,j+1]);
							if(CheckHp(map[i,j+1]) <= 0){
								if(bombOfPlayer >= 0)
								players[bombOfPlayer].killedPlayer = map[i,j+1];
							}
						}
					break;

						
				}
				
				startY += 1f;
			}
			startX += 1f;
		}
		bombsTickDown(); //All bombs counter down (guards);
		CheckDeaths();

	}

	void removeAll(){
		 GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>() ;
 		 foreach(GameObject go in allObjects)
    		if (go.layer != 8){
				Destroy(go.gameObject);
			}

	}

	//Functions to use from other scripts are here
	public int[,] GetMap(){
		return map;
	}

	public void RotateClockwise(int playerIndex){
		
		if(canMove(playerIndex)){
			players[(playerIndex / 10) - 1].lastAction = Action.RotateClockwise;
			int index = (playerIndex / 10) - 1;
			
			switch(players[index].Orientation){
				case Direction.UP:
					players[index].Orientation = Direction.RIGHT;
				break;
				case Direction.LEFT:
					players[index].Orientation = Direction.UP;
				break;
				case Direction.RIGHT:
					players[index].Orientation = Direction.DOWN;
				break;
				case Direction.DOWN:
					players[index].Orientation = Direction.LEFT;
				break;
			}
			playerMoved(playerIndex);
		}
	}

	public void RotateCounterClockwise(int playerIndex){
		
		if(canMove(playerIndex)){
			players[(playerIndex / 10) - 1].lastAction = Action.RotateCounterClockwise;
			int index = (playerIndex / 10 - 1);
			switch(players[index].Orientation){
				case Direction.UP:
					players[index].Orientation = Direction.LEFT;
				break;
				case Direction.LEFT:
					players[index].Orientation = Direction.DOWN;
				break;
				case Direction.RIGHT:
					players[index].Orientation = Direction.UP;
				break;
				case Direction.DOWN:
					players[index].Orientation = Direction.RIGHT;
				break;
			}
			playerMoved(playerIndex);
		}
	}

	private void MoveUp(int playerIndex){
		for(int i = 0; i < rowsCount; i++){
			for(int j = 0; j < columnsCount; j++){
				if((map[i,j] == playerIndex) && (map[i,j + 1] == 0 || map[i,j + 1] == 6)){
					if(canMove(playerIndex)){
						int lastTile = map[i,j - 1];
						map[i,j + 1] = playerIndex;
						players[(playerIndex / 10) - 1].y = players[(playerIndex / 10) - 1].y + 1;
						playerMoved(playerIndex);
						map[i,j] = 0;
						break;
					}
				}
			}
		}
		
	}

	private void MoveDown(int playerIndex){
		for(int i = 0; i < rowsCount; i++){
			for(int j = 0; j < columnsCount; j++){
				if((map[i,j] == playerIndex) && (map[i,j - 1] == 0 || map[i,j - 1] == 6)){
					if(canMove(playerIndex)){
						int lastTile = map[i,j - 1];
						map[i,j - 1] = playerIndex;
						players[(playerIndex / 10) - 1].y = players[(playerIndex / 10) - 1].y - 1;
						playerMoved(playerIndex);
						map[i,j] = 0;
						break;
					}
				}
			}
		}
	}

	private void MoveLeft(int playerIndex){
		for(int i = 1; i < rowsCount; i++){
			for(int j = 1; j < columnsCount; j++){
				if((map[i,j] == playerIndex) && (map[i - 1,j] == 0 || map[i-1,j] == 6)){
					if(canMove(playerIndex)){
						int lastTile = map[i - 1,j];
						map[i - 1,j] = playerIndex;
						players[(playerIndex / 10) - 1].x = players[(playerIndex / 10) - 1].x - 1;
						playerMoved(playerIndex);
						map[i,j] = 0;
						break;				
					}
				}
			}
		}
	}

	private void MoveRight(int playerIndex){
		for(int i = 0; i < rowsCount; i++){
			for(int j = 0; j < columnsCount; j++){
				if((map[i,j] == playerIndex) && (map[i + 1,j] == 0 || map[i + 1,j] == 6)){
					if(canMove(playerIndex)){
						int lastTile = map[i + 1,j];
						map[i + 1,j] = playerIndex;
						players[(playerIndex / 10) - 1].x = players[(playerIndex / 10) - 1].x + 1;
						playerMoved(playerIndex);
						map[i,j] = 0;
						break;		
					}
				}
			}
		}
	}

	public void PlaceBomb(int playerIndex){
	
		if(CheckHp(playerIndex) > 0){
			if(canMove(playerIndex)){
				players[(playerIndex / 10) - 1].lastAction = Action.PlaceBomb;
				int index = (int)(playerIndex / 10) - 1;
				int _x = players[index].x;
				int _y = players[index].y;
				Direction temp = players[index].Orientation;
				if(!isBombPlaced(playerIndex)){ //If player doesnt have bomb on a map
				
					//Is place empty for bomb
					int bombX = _x;
					int bombY = _y;
					switch(temp){
						case Direction.DOWN:
							bombY--;
						break;
						case Direction.LEFT:
							bombX--;
						break;
						case Direction.RIGHT:
							bombX++;
						break;
						case Direction.UP:
							bombY++;
						break;
					}
					if(map[bombX, bombY] == 0){
						map[bombX,bombY] = 4;
						players[index].bombPosition.x = bombX;
						players[index].bombPosition.y = bombY;
						
						players[index].bombState = 4;
					}
				}
				playerMoved(playerIndex);
			}
		}
	}

	public void MoveForward(int playerIndex){
		if(canMove(playerIndex)){
			players[(playerIndex / 10) - 1].lastAction = Action.MoveForward;
			int index = (playerIndex / 10) - 1;
			switch(players[index].Orientation){
				case Direction.DOWN:
					MoveDown(playerIndex);
				break;
				case Direction.LEFT:
					MoveLeft(playerIndex);
				break;
				case Direction.RIGHT:
					MoveRight(playerIndex);
				break;
				case Direction.UP:
					MoveUp(playerIndex);
				break;
			}
		}
	}

	public bool CanMoveForward(int playerIndex){
		int index = (playerIndex / 10) - 1;
		switch(players[index].Orientation){
			case Direction.DOWN:
			if(map[players[index].x,players[index].y-1] == 0 || map[players[index].x,players[index].y-1] == 6){
				return true;
			} else {
				return false;
			}
			case Direction.LEFT:
			if(map[players[index].x - 1,players[index].y] == 0 || map[players[index].x - 1,players[index].y] == 6){
				return true;
			} else {
				return false;
			}
			case Direction.RIGHT:
			if(map[players[index].x + 1,players[index].y] == 0 || map[players[index].x + 1,players[index].y] == 6){
				return true;
			} else {
				return false;
			}
			case Direction.UP:
			if(map[players[index].x,players[index].y+1] == 0 || map[players[index].x,players[index].y+1] == 6){
				return true;
			} else {
				return false;
			}
			
		}
		return false;
	}

	public int CheckHp(int playerIndex){
		return players[(playerIndex / 10) - 1].health;
	}

	public int GameFinished(){
		return gameWonBy;
	}

	public float GetTurnTime(){
		return turnTime;
	}


	public List<Vector2> GetBombList(){
		List<Vector2> listaBomb = new List<Vector2>();
		for(int i = 0; i < rowsCount; i++){
			for(int j = 0; j < columnsCount; j++){
				if(map[i,j] >= 1 && map[i,j] < 5){
					listaBomb.Add(new Vector2(i,j));
				}
			}
		}
		return listaBomb;
	}

	public bool isBlowingNearPlayer(int playerIndex){
		int index = (playerIndex / 10) - 1;
		int playerX = players[index].x;
		int playerY = players[index].y;

		List<Vector2> bombsPositions = GetBombList();
		for(int i = 0; i < bombsPositions.Count; i++){
			if(map[(int)bombsPositions[i].x,(int)bombsPositions[i].y] == 2){
				if((int)bombsPositions[i].x == playerX && (int)bombsPositions[i].y == playerY - 1){
					return true;
				} else if((int)bombsPositions[i].x == playerX && (int)bombsPositions[i].y == playerY + 1){
					return true;
				} else if((int)bombsPositions[i].x == playerX - 1 && (int)bombsPositions[i].y == playerY){
					return true;
				} else if((int)bombsPositions[i].x == playerX + 1 && (int)bombsPositions[i].y == playerY){
					return true;
				}
			}
		}
		return false;
	}

	public bool isBombNearPlayer(int playerIndex){
		int index = (playerIndex / 10) - 1;
		int playerX = players[index].x;
		int playerY = players[index].y;

		List<Vector2> bombsPositions = GetBombList();
		for(int i = 0; i < bombsPositions.Count; i++){
			if(map[(int)bombsPositions[i].x,(int)bombsPositions[i].y] >= 1 && map[(int)bombsPositions[i].x,(int)bombsPositions[i].y] < 6){
				if((int)bombsPositions[i].x == playerX && (int)bombsPositions[i].y == playerY - 1){
					return true;
				} else if((int)bombsPositions[i].x == playerX && (int)bombsPositions[i].y == playerY + 1){
					return true;
				} else if((int)bombsPositions[i].x == playerX - 1 && (int)bombsPositions[i].y == playerY){
					return true;
				} else if((int)bombsPositions[i].x == playerX + 1 && (int)bombsPositions[i].y == playerY){
					return true;
				}
			}
		}
		return false;
	}

	public int whosBombItIs(int x, int y){ //zwraca indeks
		for(int i = 0; i < 3; i++){
			if(players[i].bombPosition.x == x && players[i].bombPosition.y == y){
				return i;
			}
		} 
		return -1;
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

	private void CheckDeaths(){
		int howManyAlive = 0;
		for(int i = 0; i < 3; i++){
			if(players[i].health == 0){
				map[players[i].x, players[i].y] = 0;
				players[i].health = -1; //Doesnt exist
			} else if(players[i].health == -1){
				howManyAlive++;
			}
		}

		if(howManyAlive >= 2){
			for(int i = 0; i < 3; i++){
				if(players[i].health > 0){
					gameWonBy = (i + 1) * 10;
				}
			}
			//Debug.log("Koniec parti");
		}
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

	private Quaternion setRotation(int playerIndex){
		
		Quaternion x = Quaternion.identity;
		int index = (playerIndex / 10) - 1;
		switch(players[index].Orientation){
			case Direction.DOWN:
				x = Quaternion.Euler(0,90,0);
			break;
			case Direction.LEFT:
				x = Quaternion.Euler(0,-180,0);
			break;
			case Direction.RIGHT:
				x = Quaternion.Euler(0,0,0);
			break;
			case Direction.UP:
				x = Quaternion.Euler(0,-90,0);
			break;
		}
		return x;

	}

	private bool isBombPlaced(int playerIndex){ //Is player bomb placed already?
		if(players[(int)(playerIndex / 10) - 1].bombState <= 0){
			return false;
		} else {
			return true;
		}
	}


	private void bombsTickDown(){
		foreach(Player p in players){
			if(p.bombState > 0 && p.bombState < 5){
				p.bombState--;
			}
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
		players[(playerIndex / 10) - 1].health--;
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




	public List<Action> aStar(Vector2 start, Direction startDir, Vector2 end, Direction endDir){
		if(map[(int)end.x, (int)end.y] == 0){

		Node start_node = new Node(null, start, startDir);
		Node end_node = new Node(null, end, endDir);

		List<Node> open_list = new List<Node>(); 
		List<Node> closed_list = new List<Node>();

		open_list.Add(start_node);
		while(open_list.Count > 0){
			//Get current node
			Node current_node = open_list[0];
			int current_index = 0;
			for(int i = 0; i < open_list.Count; i++){
				if(open_list[i].f <= current_node.f){
					current_node = open_list[i];
					current_index = i;
				}
			}

			//Pop current element and add it to closed list
			open_list.RemoveAt(current_index);
			closed_list.Add(current_node);

			//If found a goal
			if(current_node.position == end_node.position){
				List<Action> path = new List<Action>();
				Node current = current_node;
				Vector2 lastPosition = new Vector2(-1,-1);
				Direction lastDir = Direction.DOWN;
				int counter = 0;
				while(current != null){
					if(counter != 0){
						if(lastPosition != current.position){
							path.Add(Action.MoveForward);
						} else {
							if(lastDir == Direction.LEFT){
								if(current.orientation == Direction.UP){
									path.Add(Action.RotateCounterClockwise);
								} else {
									path.Add(Action.RotateClockwise);
								}
							} else if(lastDir == Direction.RIGHT){
								if(current.orientation == Direction.UP){
									path.Add(Action.RotateClockwise);
								} else {
									path.Add(Action.RotateCounterClockwise);
								}
							} else if(lastDir == Direction.UP){
								if(current.orientation == Direction.RIGHT){
									path.Add(Action.RotateCounterClockwise);
								} else {
									path.Add(Action.RotateClockwise);
								}
							} else if(lastDir == Direction.DOWN){
								if(current.orientation == Direction.RIGHT){
									path.Add(Action.RotateClockwise);
								} else {
									path.Add(Action.RotateCounterClockwise);
								}
							} 
						}

					} 
					//path.Add(current.position);
					
					lastPosition = current.position;
					lastDir = current.orientation;
					current = current.parent;
					counter++;
				}
				path.Reverse();
				return path;
			}

			//Children generating
			List<Node> children = new List<Node>();
			List<Vector2> new_position = new List<Vector2>();
			//All possible moves
			new_position.Add(new Vector2(1,0));
			new_position.Add(new Vector2(-1,0));
			new_position.Add(new Vector2(0,1));
			new_position.Add(new Vector2(0,-1));
			Vector2 node_position = new Vector3(0,0,0);
			Node new_node = new Node();
			switch(current_node.orientation){
				case Direction.DOWN:
					node_position = new Vector2(current_node.position.x + new_position[3].x, current_node.position.y + new_position[3].y);
					new_node = new Node(current_node, current_node.position, Direction.LEFT);
					children.Add(new_node);
					new_node = new Node(current_node, current_node.position, Direction.RIGHT);
					children.Add(new_node);
				break;
				case Direction.LEFT:
					node_position = new Vector2(current_node.position.x + new_position[1].x, current_node.position.y + new_position[1].y);
					new_node = new Node(current_node, current_node.position, Direction.UP);
					children.Add(new_node);
					new_node = new Node(current_node, current_node.position, Direction.DOWN);
					children.Add(new_node);
				break;
				case Direction.RIGHT:
					node_position = new Vector2(current_node.position.x + new_position[0].x, current_node.position.y + new_position[0].y);
					new_node = new Node(current_node, current_node.position, Direction.UP);
					children.Add(new_node);
					new_node = new Node(current_node, current_node.position, Direction.DOWN);
					children.Add(new_node);
				break;
				case Direction.UP:
					node_position = new Vector2(current_node.position.x + new_position[2].x, current_node.position.y + new_position[2].y);
					new_node = new Node(current_node, current_node.position, Direction.LEFT);
					children.Add(new_node);
					new_node = new Node(current_node, current_node.position, Direction.RIGHT);
					children.Add(new_node);
				break;
			}
			if(map[(int)node_position.x, (int)node_position.y] == 0){
				new_node = new Node(current_node, node_position, current_node.orientation);
				children.Add(new_node);
			} else if(map[(int)node_position.x, (int)node_position.y] == 6){
				new_node = new Node(current_node, node_position, current_node.orientation);
				new_node.waterPenalty += 25;
				children.Add(new_node);
			}

			

			/* 
			for(int i = 0; i < new_position.Count; i++){
				Vector2 node_position = new Vector2(current_node.position.x + new_position[i].x, current_node.position.y + new_position[i].y);

				//Check obstacles
				if(map[(int)node_position.x, (int)node_position.y] != 0){
					continue;
				}


				//Creating new node
				Node new_node = new Node(current_node, node_position);
				children.Add(new_node);
			} */


			//Loop through all childrens (possible moves)
			for(int i = 0;i < children.Count; i++){

				bool toAdd = true;
				//Check if child is on closed list already
				for(int j = 0; j < closed_list.Count; j++){
					if((children[i].position == closed_list[j].position) && (children[i].orientation == closed_list[j].orientation)){
						toAdd = false;
					}
				}

				//Counting g h f
				children[i].g = current_node.g + 1 + current_node.waterPenalty;
				children[i].h = (Mathf.Pow(children[i].position.x - end_node.position.x,2) + Mathf.Pow(children[i].position.y - end_node.position.y,2));
				children[i].f = children[i].g + children[i].h;

				//If child is already on open list
				for(int j = 0; j < open_list.Count; j++){
					if(((children[i].position == open_list[j].position)&&(children[i].orientation == open_list[j].orientation)) && (children[i].g > open_list[j].g)){
						toAdd = false;
					}
				}

				if(toAdd == true){
					open_list.Add(children[i]);
				}

			}

		}
		return null;
		} else {
			//Debug.logWarning("U cant move there...");
		}
		return null;

	}


		public void ShowError(int akcja){
			if(gameWonBy <= -2)
				Debug.Log("ERR" + akcja.ToString());
		}
























		public void ShowErrors(int akcja){
		if(isBombNearPlayer(10)){
			Debug.Log("BOMB");
			if(CanMoveForward(10)){
				MoveForward(10);
			} else {
				RotateClockwise(10);
			}
		} else {
			if(akcja == 0 || akcja == 3){
				if(!CanMoveForward(10)){
					int random = Random.Range(0,2);
					if(random == 0){
						RotateClockwise(10);
					} else {
						RotateCounterClockwise(10);
					}
				}
			} 
		}
	}

}

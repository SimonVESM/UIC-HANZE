using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace InnerDriveAcademy.TownsVille
{

	/**
	 * GameManager is the class that manages the whole flow of the game for us. 
	 * For a real/bigger game, this might be the responsibity of multiple classes,
	 * for a game this size, 1 simple manager works perfectly fine.
	 */
	public class GameManager : MonoBehaviour
	{
		//Level settings, these kind of settings should be defined in a file or Scriptable objects,
		//but in this demo which only has a single 'playthrough', we are simply putting them here:
		[Header("Demo Level settings")]

		public UFO ufoPrefab;
		[Range(1, 30)] public float timeBetweenUFOSpawns = 1;      //time in seconds
		[Range(15, 200)] public float ufoSpawnHeight = 50;           //how far above the town are the ufo's spawned?
		[Range(1, 30)] public int ufosToDestroyCount = 10;          //how many ufo's do we have to destroy to win the game?

		[Header("Debug settings")]
		public bool autoStart = false;                                  //immediately start the game when we press play (for testing purposes)
		public bool testMode = false;                                   //test mode speeds everything up
		[Range(0.1f, 1)] public float testUfoSpawnTimeMultiplier = 0.1f;//reduces the spawn time in test mode
		[Range(0.1f, 1)] public float testHouseHealthMultiplier = 0.1f; //reduces the house health in test mode

		[Header("HUD fields")]
		public TMP_Text housesLeftText;
		public TMP_Text ufosLeftText;
		public TMP_Text gameStatusText;

		[Header("GameManager events")]
		public UnityEvent OnGameInitialized;
		public UnityEvent OnGameStarted;
		public UnityEvent OnGameOver;

		//When we initialize the game we look up all houses. 
		//After that we don't destroy them, we simply disable them so we can restart the level ;)
		private List<House> housesInTheScene;

		//Administration during game play
		private int ufosLeftCount = 0;                  //if this reaches 0, we've won the game
		private int housesLeftCount = 0;                //if this reaches 0, we've lost the game
		private bool inPlay = false;                    //used to validate state while playing
		private List<House> availableHousesToTarget;    //so we can pick a random house to target by a ufo
		private List<UFO> activeUFOs;                   //so we can despawn active UFO's when the game ends

		private void Start()
		{
			setGameStatusText("");
			//Get all the houses in the Scene and print some info about them
			housesInTheScene = FindObjectsOfType<House>().ToList();

			if (housesInTheScene.Count == 0)
			{
				Debug.Log("No houses found in the Scene, did you forget to add a House Script to the House prefab?");
			}
			else
			{
				Debug.Log(housesInTheScene.Count + " houses found to destroy :)");
			}

			OnGameInitialized.Invoke();

			if (autoStart) StartGame();
		}

		[ContextMenu("Start game")]
		public void StartGame()
		{
			if (inPlay || !Application.isPlaying)
			{
				Debug.Log("Invalid game play state to call this method");
				return;
			}

			if (ufoPrefab == null)
			{
				Debug.Log("Please assign a UFO prefab to the GameManager before starting the game.");
				return;
			}

			//Start a coroutine, coroutines run parallel to the main thread as a kind of pseudo thread
			StartCoroutine(playGame());
		}

		private IEnumerator playGame()
		{
			OnGameStarted.Invoke();

			//Reset the available targets and administration
			foreach (House house in housesInTheScene) house.ResetHouse(testMode ? testHouseHealthMultiplier : 1);

			availableHousesToTarget = new List<House>(housesInTheScene);
			activeUFOs = new List<UFO>();
			setUFOsLeftCount(ufosToDestroyCount);
			setHousesLeftCount(availableHousesToTarget.Count);
			setGameStatusText("");

			//listen to events from both UFO's and Houses
			UFO.OnDestroyed += OnUFODestroyed;
			UFO.OnDespawned += OnUFODespawned;
			House.OnDestroyed += OnHouseDestroyed;

			inPlay = true;

			while (inPlay)
			{
				//Debug.Log("Waiting to spawn UFO");
				yield return new WaitForSeconds(timeBetweenUFOSpawns * (testMode ? testUfoSpawnTimeMultiplier : 1));

				//If it is time to spawn a UFO, get a random house and spawn a UFO at that house
				if (availableHousesToTarget.Count > 0 && ufosLeftCount > 0)
				{
					//Pick a random house and remove it from the current list of houses to target
					Debug.Log("Spawning ufo");
					House randomHouse = availableHousesToTarget[Random.Range(0, availableHousesToTarget.Count)];
					availableHousesToTarget.Remove(randomHouse);

					UFO ufoInstance = Instantiate(ufoPrefab, randomHouse.transform.position + Vector3.up * ufoSpawnHeight + Vector3.up * Random.value, Quaternion.identity);
					//Set up the UFO to attack this random house, do not assign the ufo as an attacker of the house
					//yet, since the UFO might want to spawn in or something like that first
					activeUFOs.Add(ufoInstance);
					ufoInstance.target = randomHouse;
				}
				else
				{
					//Debug.Log("No more houses left to attack.");
				}
			}
		}

		private void OnHouseDestroyed(House pHouse)
		{
			//If the house is destroyed, update our house destroyed count 
			setHousesLeftCount(housesLeftCount - 1);
			//Make sure, the house wasn't just readded in case this was the same frame in which
			//we destroyed the UFO targeting this house.
			availableHousesToTarget.Remove(pHouse);
			Debug.Log("Houses left:" + housesLeftCount);
			if (housesLeftCount == 0) gameLost();
		}

		private void OnUFODestroyed(UFO pUFO)
		{
			setUFOsLeftCount(ufosLeftCount - 1);

			activeUFOs.Remove(pUFO);
			//the target house becomes available for another UFO to target
			availableHousesToTarget.Add(pUFO.target);
			//print some debug info
			Debug.Log("Available houses:" + availableHousesToTarget.Count);
			Debug.Log("UFOs left to win:" + ufosLeftCount);

			if (ufosLeftCount == 0) gameWon();
		}

		private void OnUFODespawned(UFO pUFO)
		{
			activeUFOs.Remove(pUFO);
		}

		private void gameWon()
		{
			gameOver();
			setGameStatusText("You won! Townsville is saved!");
		}

		private void gameLost()
		{
			gameOver();
			setGameStatusText("Bye bye townsville, you've lost!");
		}

		private void gameOver()
		{
			inPlay = false;
			StopAllCoroutines();

			//make sure we are no longer listening to any of the events ...
			UFO.OnDestroyed -= OnUFODestroyed;
			UFO.OnDespawned -= OnUFODespawned;
			House.OnDestroyed -= OnHouseDestroyed;

			//... before we do our housekeeping
			foreach (UFO ufo in activeUFOs) ufo.Despawn();

			OnGameOver.Invoke();
		}

		private void setUFOsLeftCount(int pUFOsLeftCount)
		{
			ufosLeftCount = pUFOsLeftCount;
			if (ufosLeftText != null) ufosLeftText.text = "" + ufosLeftCount;
		}

		private void setHousesLeftCount(int pHousesLeftCount)
		{
			housesLeftCount = pHousesLeftCount;
			if (housesLeftText != null) housesLeftText.text = "" + housesLeftCount;
		}

		private void setGameStatusText(string pStatus)
		{
			if (gameStatusText != null)
			{
				gameStatusText.text = pStatus;
			}
			else
			{
				Debug.Log(pStatus);
			}
		}

	}
}
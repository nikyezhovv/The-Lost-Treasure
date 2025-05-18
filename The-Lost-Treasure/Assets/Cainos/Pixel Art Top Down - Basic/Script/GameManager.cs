using Cainos.PixelArtPlatformer_Dungeon;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int spawnIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Уже есть экземпляр
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void IncrementSpawnIndex(int maxSpawns)
    {
        spawnIndex = (spawnIndex + 1) % maxSpawns;
    }
}


//public class GameManager : MonoBehaviour
//{
//    public static GameManager Instance;

//    public enum DoorState { Closed, Open }

//    public DoorState door1 = DoorState.Open;  // В начале открыта 1 дверь
//    public DoorState door2 = DoorState.Closed;
//    public DoorState door3 = DoorState.Closed;


//    public Object odoor1;
//    public Object odoor2;
//    public Object odoor3;
//    public int currentScene = 0;

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Debug.Log("Awake1");
//            Instance = this;
//            DontDestroyOnLoad(gameObject);

//        }
//        else
//        {
//            Debug.Log("Awake2");
//            Destroy(gameObject);
//        }

//        SceneManager.sceneLoaded += OnSceneLoaded;
//    }

//    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//    {
//        var sceneName = scene.rootCount;
//        if (sceneName == 0)
//        {
//            currentScene += 1;
//            SetScene();
//            // Выводим в консоль
//            Debug.Log($"Сцена {sceneName} загружена {currentScene} раз(а).");
//        }
//    }

//    public void SetScene()
//    {
//        Debug.Log("---scene Number" + currentScene);
//       // currentScene += 1;

//        // Логика обновления дверей при смене сцены
//        switch (currentScene)
//        {
//            case 1:
//                door1 = DoorState.Open;
//                door2 = DoorState.Closed;
//                door3 = DoorState.Closed;
//                break;
//            case 2:
//                door1 = DoorState.Closed;
//                door2 = DoorState.Open;
//                door3 = DoorState.Closed;
//                break;
//            case 3:
//                door1 = DoorState.Closed;
//                door2 = DoorState.Closed;
//                door3 = DoorState.Open;
//                break;
//        }
//    }

//    public bool IsDoorOpen(int doorNumber)
//    {
//        Debug.Log("--- dore: " + doorNumber + "sceen" + currentScene);
//        Debug.Log(door1 == DoorState.Open);
//        Debug.Log(door2 == DoorState.Open);
//        Debug.Log(door3 == DoorState.Open);
//        return doorNumber switch
//        {
//            1 => door1 == DoorState.Open,
//            2 => door2 == DoorState.Open,
//            3 => door3 == DoorState.Open,
//            _ => false
//        };
//    }
//}
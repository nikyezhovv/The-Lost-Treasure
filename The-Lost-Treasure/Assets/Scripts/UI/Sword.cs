using System.IO;
using UnityEngine;

public class SwordPickup : MonoBehaviour
{
    public string fileName = "playerData.json";
    public RuntimeAnimatorController animatorWithSword;
    [SerializeField] private GameObject[] unlockOnTAke;


    private bool isPlayerNear = false;
    private ChestOpener chestOpener;

    void Start()
    {
        chestOpener = FindObjectOfType<ChestOpener>(); // получаем ссылку на сундук
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            GiveSwordToPlayer();
        }
    }

    void GiveSwordToPlayer()
    {
        foreach (var obj in unlockOnTAke)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && animatorWithSword != null)
        {
            player.GetComponent<Animator>().runtimeAnimatorController = animatorWithSword;
            SaveWeapon("sword");

            // Найти объект сундука и уведомить его
            //ChestOpener chest = FindObjectOfType<ChestOpener>();
            //if (chest != null)
            //{
            //    chest.OnSwordPickedUp();
            //}
        }

        gameObject.SetActive(false); // убрать меч из сцены
    }

    private void SaveWeapon(string weaponName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            data.weapon = weaponName;
            File.WriteAllText(path, JsonUtility.ToJson(data, true));
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerNear = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerNear = false;
    }

    [System.Serializable]
    public class PlayerData
    {
        public int health;
        public int level;
        public string weapon;
    }
}
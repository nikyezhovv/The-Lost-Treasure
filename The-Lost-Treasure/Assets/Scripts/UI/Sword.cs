using System.IO;
using UnityEngine;

public class SwordPickup : MonoBehaviour
{
    public string fileName = "playerData.json";
    public RuntimeAnimatorController animatorWithSword;
    [SerializeField] private GameObject[] unlockOnTAke;


    private bool _isPlayerNear;
    private ChestOpener _chestOpener;

    private void Start()
    {
        _chestOpener = FindObjectOfType<ChestOpener>();
    }

    private void Update()
    {
        if (_isPlayerNear && Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            GiveSwordToPlayer();
        }
    }

    private void GiveSwordToPlayer()
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
        }

        gameObject.SetActive(false);
    }

    private void SaveWeapon(string weaponName)
    {
        var path = Path.Combine(Application.persistentDataPath, fileName);

        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            var data = JsonUtility.FromJson<PlayerData>(json);
            data.weapon = weaponName;
            File.WriteAllText(path, JsonUtility.ToJson(data, true));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _isPlayerNear = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _isPlayerNear = false;
    }

    [System.Serializable]
    public class PlayerData
    {
        public int health;
        public int level;
        public string weapon;
    }
}
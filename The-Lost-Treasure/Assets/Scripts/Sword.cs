using System.IO;
using UnityEngine;

public class SwordPickup : MonoBehaviour
{
    public string fileName = "playerData.json";
    public RuntimeAnimatorController animatorWithSword;
    private bool isPlayerNear = false;

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            GiveSwordToPlayer();
        }
    }

    void GiveSwordToPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && animatorWithSword != null)
        {
            player.GetComponent<Animator>().runtimeAnimatorController = animatorWithSword;
            SaveWeapon("sword");
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
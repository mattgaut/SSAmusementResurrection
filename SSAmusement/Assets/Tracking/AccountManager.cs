using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class AccountManager : Singleton<AccountManager> {

    static string file_path = "Accounts/";
    static string account_file_extension = ".ssa";
    static string meta_file_extension = ".ssmeta";

    Account current_account;

    [SerializeField] StatisticTrackerManager statistics;
    [SerializeField] AchievementManager achievements;

    public void LoadOrCreateAccount(string name) {
        if (!LoadAccount(name)) {
            CreateAccount(name);
        }
    }

    public bool LoadAccount(string name) {
        if (current_account != null) {
            SaveAccount();
        }

        if (File.Exists(Application.persistentDataPath + file_path + name + account_file_extension)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + file_path + name + account_file_extension, FileMode.Open);
            Account account_data = (Account)bf.Deserialize(file);
            file.Close();

            current_account = new Account(account_data);

            achievements.LoadData(current_account.achievements);
            statistics.LoadData(current_account.stats);

            return true;
        }

        return false;
    }

    public bool CreateAccount(string name) {
        if (current_account != null) {
            SaveAccount();
        }

        if (!File.Exists(Application.persistentDataPath + file_path + name + account_file_extension)) {
            current_account = new Account(name);
            ResetAccount();
            return true;
        }

        return true;
    }


    public void ResetAccount() {
        current_account = new Account(current_account.name);

        statistics.LoadData(current_account.stats);
        achievements.LoadData(current_account.achievements);

        SaveAccount();
    }

    public bool DeleteAccount(string name) {
        if (File.Exists(Application.persistentDataPath + file_path + name + account_file_extension)) {
            File.Delete(Application.persistentDataPath + file_path + name + account_file_extension);
            return true;
        }
        return false;
    }

    public void SaveAccount() {
        if (!Directory.Exists(Application.persistentDataPath + file_path)) {
            Directory.CreateDirectory(Application.persistentDataPath + file_path);
        }

        UpdateAccount();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + file_path + current_account.name + account_file_extension);
        bf.Serialize(file, current_account);
        file.Close();
    }

    protected void Start() {
        LoadOrCreateAccount("Account");
    }

    private void UpdateAccount() {
        current_account.achievements = achievements.GetData();
        current_account.stats = statistics.GetData();
    }

    private void OnDisable() {
        SaveAccount();
    }
}

[System.Serializable]
public class Account {
    public AchievementManager.Data achievements;
    public StatisticTrackerManager.Data stats;

    public string name {
        get { return _name; }
    }

    [SerializeField] string _name;

    public Account(string name) {
        _name = name;
        achievements = new AchievementManager.Data(new List<Achievement>());
        stats = new StatisticTrackerManager.Data(new List<Statistic>());
    }

    public Account(string name, AchievementManager.Data a_data, StatisticTrackerManager.Data stat_data) {
        _name = name;
        achievements = a_data;
        stats = stat_data;
    }

    public Account(Account other) {
        _name = other.name;
        achievements = other.achievements;
        stats = other.stats;
    }
}

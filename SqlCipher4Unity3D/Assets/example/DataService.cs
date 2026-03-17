using System.Collections.Generic;
using SqlCipher4Unity3D;
using UnityEngine;

#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Networking;
#endif
namespace example
{
    public class DataService
    {
        private readonly SQLiteConnection _connection;

        public DataService(string DatabaseName)
        {
#if UNITY_EDITOR
            string dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
            // check if file exists in Application.persistentDataPath
            string filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

            if (!File.Exists(filepath))
            {
                Debug.Log("Database not in Persistent path");
                // if it doesn't ->
                // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID
                using (UnityWebRequest loadDb = UnityWebRequest.Get("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName))
                {
                    loadDb.SendWebRequest();
                    float timeout = 10f;
                    float elapsed = 0f;
                    while (!loadDb.isDone)
                    {
                        System.Threading.Thread.Sleep(10);
                        elapsed += 0.01f;
                        if (elapsed >= timeout)
                            throw new System.Exception("Timed out loading database from Android assets: " + DatabaseName);
                    }
                    if (loadDb.result != UnityWebRequest.Result.Success)
                        throw new System.Exception("Failed to load database from Android assets: " + loadDb.error);
                    // then save to Application.persistentDataPath
                    File.WriteAllBytes(filepath, loadDb.downloadHandler.data);
                }
#elif UNITY_IOS
                string loadDb =
     Application.dataPath + "/Raw/" + DatabaseName; // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy (loadDb, filepath);
#elif UNITY_WP8
                string loadDb =
     Application.dataPath + "/StreamingAssets/" + DatabaseName; // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy (loadDb, filepath);
    
#elif UNITY_WINRT
                string loadDb =
     Application.dataPath + "/StreamingAssets/" + DatabaseName; // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy (loadDb, filepath);
#elif UNITY_STANDALONE_OSX
                string loadDb =
     Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName; // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#else
                string loadDb =
     Application.dataPath + "/StreamingAssets/" + DatabaseName; // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#endif

                Debug.Log("Database written");
            }

            var dbPath = filepath;
#endif
            _connection = new SQLiteConnection(dbPath, "password");
            Debug.Log("Final PATH: " + dbPath);
        }

        public void CreateDB()
        {
            _connection.DropTable<Person>();
            _connection.CreateTable<Person>();

            _connection.InsertAll(new[]
            {
                new Person
                {
                    Id = 1,
                    Name = "Tom",
                    Surname = "Perez",
                    Age = 56
                },
                new Person
                {
                    Id = 2,
                    Name = "Fred",
                    Surname = "Arthurson",
                    Age = 16
                },
                new Person
                {
                    Id = 3,
                    Name = "John",
                    Surname = "Doe",
                    Age = 25
                },
                new Person
                {
                    Id = 4,
                    Name = "Roberto",
                    Surname = "Huertas",
                    Age = 37
                }
            });
        }

        public IEnumerable<Person> GetPersons()
        {
            return _connection.Table<Person>();
        }

        public IEnumerable<Person> GetPersonsNamedRoberto()
        {
            return _connection.Table<Person>().Where(x => x.Name == "Roberto");
        }

        public Person GetJohnny()
        {
            return _connection.Table<Person>().Where(x => x.Name == "Johnny").FirstOrDefault();
        }

        public Person CreatePerson()
        {
            Person p = new Person
            {
                Name = "Johnny",
                Surname = "Mnemonic",
                Age = 21
            };
            _connection.Insert(p);
            return p;
        }
    }
}
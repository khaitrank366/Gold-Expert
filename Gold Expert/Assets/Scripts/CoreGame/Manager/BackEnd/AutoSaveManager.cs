using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class AutoSaveManager : Singleton<AutoSaveManager>
{
    private const float AUTO_SAVE_INTERVAL = 5f;
    private Coroutine autoSaveCoroutine;
    private bool isInitialized = false;

    private void Start()
    {
        Initialize();
    }

    private void OnDestroy()
    {
        StopAutoSave();
    }

    public void Initialize()
    {
        if (isInitialized) return;
        StartAutoSave();
        isInitialized = true;
    }

    private void StartAutoSave()
    {
        if (autoSaveCoroutine != null)
        {
            StopCoroutine(autoSaveCoroutine);
        }
        autoSaveCoroutine = StartCoroutine(AutoSaveRoutine());
        Debug.Log("✅ Auto save system started");
    }

    private void StopAutoSave()
    {
        if (autoSaveCoroutine != null)
        {
            StopCoroutine(autoSaveCoroutine);
            autoSaveCoroutine = null;
            Debug.Log("🛑 Auto save system stopped");
        }
    }

    private IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(AUTO_SAVE_INTERVAL);
            AutoSave().ContinueWith(task => 
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"❌ Auto save failed: {task.Exception?.InnerException?.Message}");
                }
            });
        }
    }

    private async Task AutoSave()
    {
        try
        {
            // Update LastOnline
            PlayFabManager.Instance.DataDictionary[Common.PlayerDataKeyHelper.ToKey(PlayerDataKey.LastOnline)] = 
                System.DateTime.UtcNow.ToString("o");

            // // Save all data
            // await PlayFabManager.Instance.SaveAll();
            // Debug.Log($"💾 Auto saved at {DateTime.Now:HH:mm:ss}");
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Auto save failed: {e.Message}");
            throw;
        }
    }

    #region Public Methods
    public void PauseAutoSave()
    {
        StopAutoSave();
        Debug.Log("⏸️ Auto save paused");
    }

    public void ResumeAutoSave()
    {
        StartAutoSave();
        Debug.Log("▶️ Auto save resumed");
    }

    public void ForceSave()
    {
        AutoSave().ContinueWith(task => 
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"❌ Force save failed: {task.Exception?.InnerException?.Message}");
            }
        });
    }
    #endregion
} 
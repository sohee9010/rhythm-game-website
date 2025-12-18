using UnityEngine;
using System.Collections.Generic;

public class NoteSpawner : MonoBehaviour
{
    [Header("Note Settings")]
    public GameObject notePrefab;
    public Transform[] spawnPoints;
    public float noteSpeed = 5f;

    [Header("Beatmap")]
    public BeatmapData beatmap;
    private int currentNoteIndex = 0;

    [Header("Timing")]
    public float songPosition = 0f;
    public float songBPM = 120f;
    private bool isSpawning = false;

    private void Awake()
    {
        // [안전장치] SpawnPoints가 연결 안 되어 있으면 자동으로 찾기
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            List<Transform> points = new List<Transform>();
            // 4개 레인 가정
            for (int i = 0; i < 4; i++)
            {
                GameObject obj = GameObject.Find($"SpawnPoint{i}");
                if (obj == null)
                {
                    // 없으면 임시로 생성
                    obj = new GameObject($"SpawnPoint{i}");
                    obj.transform.position = new Vector3(-1.5f + i, 6f, 0f); // 대략적인 위치
                }
                points.Add(obj.transform);
            }
            spawnPoints = points.ToArray();
        }
    }

    private void Start()
    {
        // 테스트용 비트맵 생성 (게임 시작 시 한 번만)
        GenerateTestBeatmap();
    }

    private void Update()
    {
        if (!GameManager.Instance.isPlaying || GameManager.Instance.isPaused) return;

        songPosition += Time.deltaTime;

        if (isSpawning)
        {
            CheckAndSpawnNotes();
        }
    }

    public void StartSpawning()
    {
        isSpawning = true;
        currentNoteIndex = 0;
    }

    private void CheckAndSpawnNotes()
    {
        if (beatmap == null || beatmap.notes == null) return;

        while (currentNoteIndex < beatmap.notes.Count)
        {
            NoteData noteData = beatmap.notes[currentNoteIndex];
            float spawnTime = noteData.time - 2f;

            if (songPosition >= spawnTime)
            {
                SpawnNote(noteData);
                currentNoteIndex++;
            }
            else break;
        }
    }

    private void SpawnNote(NoteData noteData)
    {
        if (noteData.lane < 0 || noteData.lane >= spawnPoints.Length) return;

        // [중요] 구멍이 비어있으면 에러 없이 넘어가는 안전장치
        if (spawnPoints[noteData.lane] == null)
        {
            // 로그는 한 번만 뜨게 하거나 생략 가능하지만, 일단 놔둠
            // Debug.LogError($"Lane {noteData.lane}의 위치(Spawn Point)가 비어있습니다!"); 
            return;
        }

        if (notePrefab == null)
        {
             Debug.LogError("Note Prefab이 없습니다!");
             return;
        }

        GameObject noteObj = Instantiate(notePrefab, spawnPoints[noteData.lane].position, Quaternion.identity);
        Note note = noteObj.GetComponent<Note>();

        if (note != null)
        {
            note.Initialize(noteData.lane, noteData.time, noteSpeed);
        }
    }

    [ContextMenu("Generate Test Beatmap")]
    public void GenerateTestBeatmap()
    {
        if (beatmap == null) beatmap = new BeatmapData();
        beatmap.notes.Clear();

        float currentTime = 2.0f;
        for (int i = 0; i < 50; i++)
        {
            int randomLane = Random.Range(0, 4);
            beatmap.notes.Add(new NoteData { lane = randomLane, time = currentTime });
            currentTime += 0.5f;
        }
        Debug.Log("테스트 비트맵 생성 완료: 노트 50개");
    }
}

[System.Serializable]
public class BeatmapData
{
    public string songName;
    public float bpm;
    public List<NoteData> notes = new List<NoteData>();
}

[System.Serializable]
public class NoteData
{
    public int lane;
    public float time;
}
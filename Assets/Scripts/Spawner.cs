using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Figure[] _allFigures;
    [SerializeField] private Transform[] _spawnPoint;
    [SerializeField] private float _spawnDelay = 0.5f;
    [SerializeField] private int _figuresPerType = 3;

    private List<GameObject> _figuresOnScene = new List<GameObject>();
    private List<GameObject> _allActiveFigures = new List<GameObject>();

    private int _initialFiguresCount;

    private void Start()
    {
        _initialFiguresCount = _allFigures.Length * _figuresPerType;
        SpawnInitialFigures(_initialFiguresCount);
        EventManager.OnObjectSelected.AddListener(ObjectToActionBar);
    }

    private void ObjectToActionBar(Figure value)
    {
        _figuresOnScene.Remove(value.gameObject);

        if (_figuresOnScene.Count == 0)
        {
            EventManager.OnGameWin.Invoke();
        }
    }

    public void ReshuffleFigures()
    {
        int figuresCount = _figuresOnScene.Count;

        foreach (var figure in _figuresOnScene)
        {
            if (figure != null)
            {
                _allActiveFigures.Remove(figure);
                Destroy(figure);
            }
        }
        _figuresOnScene.Clear();

        StartCoroutine(SpawnFiguresRoutine(figuresCount));
    }

    private void SpawnInitialFigures(int count) => StartCoroutine(SpawnFiguresRoutine(count));

    private IEnumerator SpawnFiguresRoutine(int count)
    {
        count = Mathf.Min(count, _allFigures.Length * _figuresPerType);

        List<Figure> figuresToSpawn = new List<Figure>();
        List<Figure> tempFigures = new List<Figure>(_allFigures);

        while (figuresToSpawn.Count < count)
        {
            Shuffle(tempFigures);

            foreach (var figure in tempFigures)
            {
                if (figuresToSpawn.Count >= count) break;

                int existingCount = CountFigureInList(_allActiveFigures, figure);
                int plannedCount = CountFigureInList(figuresToSpawn, figure);

                if (existingCount + plannedCount < _figuresPerType)
                {
                    figuresToSpawn.Add(figure);
                }
            }
        }

        Shuffle(figuresToSpawn);

        foreach (var figure in figuresToSpawn)
        {
            SpawnFigure(figure);
            yield return new WaitForSeconds(_spawnDelay);
        }
    }

    private void SpawnFigure(Figure figure)
    {
        int randomX = Random.Range(0, _spawnPoint.Length);
        GameObject newFigure = Instantiate(figure.Prefab, _spawnPoint[randomX]);
        Figure newFigureScript = newFigure.GetComponent<Figure>();
        newFigureScript.SetId(figure.ID);
        newFigureScript.Initialize();

        _figuresOnScene.Add(newFigure);
        _allActiveFigures.Add(newFigure);
    }

    private int CountFigureInList(List<GameObject> list, Figure targetFigure)
    {
        int count = 0;
        foreach (var go in list)
        {
            if (go == null) continue;
            var fig = go.GetComponent<Figure>();
            if (fig != null && fig.ID == targetFigure.ID)
            {
                count++;
            }
        }
        return count;
    }

    private int CountFigureInList(List<Figure> list, Figure targetFigure)
    {
        int count = 0;
        foreach (var fig in list)
        {
            if (fig != null && fig.ID == targetFigure.ID)
            {
                count++;
            }
        }
        return count;
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}

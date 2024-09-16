using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldspaceHealthBar : MonoBehaviour
{
    private Camera _camera;
    private Transform target;
    private Vector3 offset;
    [SerializeField] private Slider enemyHealthSlider;
    [SerializeField] private Slider easeHealthSlider;
    private float lerpSpeed = 0.01f;
    private void Start() {
        _camera = Camera.main;
        offset = new Vector3(0, 1.1f, 0);
        target = transform.parent.parent;
    }

    public void UpdateHealthBar(int currentValue, int maxValue) {
        enemyHealthSlider.value = currentValue;
        enemyHealthSlider.maxValue = maxValue;
        easeHealthSlider.maxValue = maxValue;
    }

    private void Update() {
        transform.rotation = _camera.transform.rotation;
        transform.position = target.position + offset;
        easeHealthSlider.transform.rotation = _camera.transform.rotation;
        easeHealthSlider.transform.position = target.position + offset;
        if(enemyHealthSlider.value != easeHealthSlider.value) {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, enemyHealthSlider.value, lerpSpeed);
        }
    }
}

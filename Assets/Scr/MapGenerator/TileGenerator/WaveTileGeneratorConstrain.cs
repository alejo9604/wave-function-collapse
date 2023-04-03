using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WaveTileGeneratorConstrain : MonoBehaviour
{
    [SerializeField]
    private Transform _imagesContainer;
    [SerializeField]
    private Transform _optionContainer;

    private List<Image> _images = new List<Image>();
    private int _selectedImage;
    public Action OnColorUpdated;

    public Color[] Color => _images.Select(x => x.color).ToArray();
    
    public void SetColor(Color[] c)
    {
        for (int i = 0; i < _images.Count; i++)
        {
            c[i].a = 1;
            _images[i].color = c[i];
        }
    }

    public void CreateColorImages(int amount, Image prefab)
    {
        foreach (Transform t in _imagesContainer.transform)
        {
            Destroy(t.gameObject);
        }

        _images.Clear();
        
        for (int i = 0; i < amount; i++)
        {
            int index = i;
            Image image = Instantiate(prefab, _imagesContainer);
            
            Button btn = image.GetComponent<Button>();
            btn.onClick.AddListener( () => OnSelectedImage(index));
            
            _images.Add(image);
        }

        OnSelectedImage(0);
    }
    
    public void SetOptions(List<Color> options, Image prefab)
    {
        foreach (Transform t in _optionContainer.transform)
        {
            Destroy(t.gameObject);
        }

        for (int i = 0; i < options.Count; i++)
        {
            int index = i;
            Color c = options[i];
            Image option = Instantiate(prefab, _optionContainer);
            option.color = options[i];
            
            Button btn = option.GetComponent<Button>();
            btn.onClick.AddListener( () => OnOptionSelected(index, c));
        }
    }

    private void OnSelectedImage(int index)
    {
        _selectedImage = index;
        for (int i = 0; i < _images.Count; i++)
        {
            _images[i].transform.GetChild(0).gameObject.SetActive(i == index);
        }
    }
    
    private void OnOptionSelected(int index, Color color)
    {
        if (Input.GetKey(KeyCode.A))
        {
            for (int i = 0; i < _images.Count; i++)
            {
                _images[i].color = color;
            }
        }
        else
        {
            _images[_selectedImage].color = color;
            OnSelectedImage((_selectedImage + 1) % _images.Count);
        }

        OnColorUpdated?.Invoke();
    }
}

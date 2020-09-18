# noci-icon

![0](img/0.png)
![1](img/1.png)
![2](img/2.png)  

![3](img/3.png)
![4](img/4.png)
![4](img/5.png)

A unity tool to create small random icons.

## Installation

TODO: via upm branch or simply clone

## Usage

Either as editor tool and save as sprite or at runtime

### Editor tool

<img src="https://github.com/Draudastic26/noci-icon/blob/develop/img/noci-generator.png" width="256">

### Runtime example usage

```csharp
private void Start()
{
    if (rend == null) rend = GetComponent<SpriteRenderer>();

    // Create a config
    var newConfig = new NociConfig(new Vector2Int(10, 10), 2, true);

    // If wanted, define some color. Default is white with black contour. 
    newConfig.ContourColor = Color.green;
    newConfig.CellColor = Color.gray;

    // Crate a noci instance with random seed (or pass some seed).
    noci = new Noci(config);

    // Assign generated texture to e.g. a SpriteRenderer component
    rend.sprite = noci.GetSprite();
}
```

## References

- Based on the algorithm by [yurkth](https://github.com/yurkth/sprator)

**Unity 2020.1.6f1**

using System;

[Serializable]
public class DogBreedsResponse
{
    public DogBreed[] data;
}

[Serializable]
public class DogBreed
{
    public string id;
    public DogBreedAttributes attributes;
}

[Serializable]
public class DogBreedAttributes
{
    public string name;
    public string description;
}

[Serializable]
public class DogFactsResponse
{
    public DogFact[] data;
}

[Serializable]
public class DogFact
{
    public string id;
    public DogFactAttributes attributes;
}

[Serializable]
public class DogFactAttributes
{
    public string body;
}

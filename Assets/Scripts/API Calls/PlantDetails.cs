using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlantDetails : MonoBehaviour
{
    public WeatherData weatherData;
    private WeatherInfo weatherInfo;
    public PlantNetAPI plantNetAPI;
    public TextMeshProUGUI plantInfo;
    public TextMeshProUGUI plantTip;

    private int weatherCode;

    //Dictionaries
    public Dictionary<string, string> Info = new Dictionary<string, string>()
        {
            {"Azadirachta indica", "Neem is a fast-growing tree that can reach a height of 15–20 meters. It is known for its medicinal properties." },
            {"Ficus benghalensis", "Banyan trees are large, evergreen trees that are native to the Indian subcontinent." },
            {"Ficus religiosa", "Peepal tree, also known as the Sacred Fig, is known for its religious significance in India." },
            {"Mangifera indica", "Mango trees are known for their delicious fruits and are widely cultivated in India." },
            {"Cocos nucifera", "Coconut trees are tropical palms known for their edible fruit and wide range of uses." },
            {"Ocimum tenuiflorum", "Tulsi, also known as Holy Basil, is a revered herb in India known for its medicinal properties." },
            {"Hibiscus rosa-sinensis", "Hibiscus is known for its large, colorful flowers and is commonly found in tropical regions." },
            {"Bougainvillea glabra", "Bougainvillea is known for its vibrant, colorful bracts and is commonly used in landscaping." },
            {"Jasminum sambac", "Jasmine is known for its fragrant white flowers and is often used in perfumes and teas." },
            {"Rosa indica", "Roses are known for their beautiful and fragrant flowers, widely cultivated for ornamental purposes." },
            {"Phyllanthus emblica", "Indian Gooseberry, also known as Amla, is known for its edible fruit rich in vitamin C." },
            {"Ficus microcarpa", "Chinese Banyan is known for its dense foliage and is commonly used in bonsai and landscaping." },
            {"Artocarpus heterophyllus", "Jackfruit is known for its large fruit and is commonly found in tropical regions." },
            {"Aegle marmelos", "Bael is known for its medicinal properties and its edible fruit." },
            {"Bambusoideae", "Bamboo is known for its fast growth and versatility in use." },
            {"Erythrina variegata", "Coral Tree is known for its bright red flowers and is commonly used in gardens." },
            {"Murraya koenigii", "Curry Leaf is known for its aromatic leaves and is commonly used in cooking." },
            {"Citrus limon", "Lemon trees are known for their edible fruit, rich in vitamin C." },
            {"Psidium guajava", "Guava trees are known for their sweet, edible fruit." },
            {"Carica papaya", "Papaya trees are known for their sweet, edible fruit and fast growth." },
            {"Tamarindus indica", "Tamarind trees are known for their edible fruit and shade-providing canopy." },
            {"Punica granatum", "Pomegranate trees are known for their edible fruit and ornamental flowers." },
            {"Moringa oleifera", "Moringa, commonly known as the drumstick tree, is a fast-growing, drought-resistant tree native to the Indian subcontinent. It is considered a superfood due to its rich nutritional profile." },
            {"Terminalia catappa", "Terminalia catappa, commonly known as the Indian almond or tropical almond, is a large tropical tree native to the Indian subcontinent and Southeast Asia." },
            {"Dalbergia latifolia", "Dalbergia latifolia, commonly known as Indian rosewood or sonokeling, is a species of legume native to the Indian subcontinent." },
        };

    public Dictionary<string, string> ThunderStormTip = new Dictionary<string, string>()
        {
            {"Azadirachta indica", "Neem trees can withstand thunderstorms, but it's essential to ensure proper drainage to prevent waterlogging."},
            {"Ficus benghalensis", "Banyan trees are generally resilient to thunderstorms, but it's advisable to provide support to young saplings."},
            {"Ficus religiosa", "Peepal trees can tolerate thunderstorms, but it's essential to check for any damage to branches and provide support if necessary."},
            {"Mangifera indica", "Mango trees can tolerate thunderstorms, but young saplings may need additional support."},
            {"Cocos nucifera", "Coconut trees are generally resistant to thunderstorms, but it's crucial to trim any dead or weak fronds to prevent damage."},
            {"Ocimum tenuiflorum", "Tulsi plants are susceptible to damage from thunderstorms, so it's best to provide them with shelter during severe weather."},
            {"Hibiscus rosa-sinensis", "Hibiscus plants may suffer from broken branches during thunderstorms; prune them regularly to maintain their shape and reduce wind resistance."},
            {"Bougainvillea glabra", "Bougainvillea plants are generally sturdy, but it's advisable to provide support to prevent damage to their sprawling branches."},
            {"Jasminum sambac", "Jasmine plants are relatively delicate and may need protection from strong winds during thunderstorms to prevent breakage."},
            {"Rosa indica", "Rose bushes can withstand thunderstorms, but it's essential to remove any diseased or dead branches to prevent wind damage."},
            {"Phyllanthus emblica", "Amla trees can tolerate thunderstorms, but it's important to prune them regularly to maintain their shape and reduce wind resistance."},
            {"Ficus microcarpa", "Chinese Banyan trees are generally resilient to thunderstorms, but it's advisable to provide support to young saplings."},
            {"Artocarpus heterophyllus", "Jackfruit trees may suffer from broken branches during thunderstorms; prune them regularly to maintain their shape and reduce wind resistance."},
            {"Aegle marmelos", "Bael trees can tolerate thunderstorms, but it's essential to check for any damage to branches and provide support if necessary."},
            {"Bambusoideae", "Bamboo plants are generally flexible and can withstand thunderstorms, but it's essential to remove any dead or weak culms."},
            {"Erythrina variegata", "Coral trees are generally sturdy, but it's advisable to provide support to prevent damage to their sprawling branches during thunderstorms."},
            {"Murraya koenigii", "Curry leaf plants may suffer from broken branches during thunderstorms; prune them regularly to maintain their shape and reduce wind resistance."},
            {"Citrus limon", "Lemon trees can tolerate thunderstorms, but it's important to remove any diseased or dead branches to prevent wind damage."},
            {"Psidium guajava", "Guava trees are generally resilient to thunderstorms, but it's advisable to provide support to prevent damage to their sprawling branches."},
            {"Carica papaya", "Papaya trees are relatively delicate and may need protection from strong winds during thunderstorms to prevent breakage."},
            {"Tamarindus indica", "Tamarind trees can tolerate thunderstorms, but it's essential to check for any damage to branches and provide support if necessary."},
            {"Punica granatum", "Pomegranate trees are generally sturdy, but it's advisable to provide support to prevent damage to their sprawling branches during thunderstorms."},
            {"Moringa oleifera", "Moringa trees are generally resilient to thunderstorms, but it's important to remove any diseased or dead branches to prevent wind damage."},
            {"Terminalia catappa", "Indian almond trees are generally sturdy, but it's advisable to provide support to prevent damage to their sprawling branches during thunderstorms."},
            {"Dalbergia latifolia", "Indian rosewood trees are generally resilient to thunderstorms, but it's important to remove any diseased or dead branches to prevent wind damage."},
        };

    public Dictionary<string, string> DrizzleTip = new Dictionary<string, string>()
        {
            {"Azadirachta indica", "Neem trees can benefit from light drizzles as they help in washing away dust and pests from leaves."},
            {"Ficus benghalensis", "Banyan trees usually thrive in drizzly weather conditions, but avoid overwatering to prevent root rot."},
            {"Ficus religiosa", "Peepal trees generally enjoy light drizzles, but ensure proper drainage to prevent waterlogging."},
            {"Mangifera indica", "Mango trees can tolerate light drizzles, but avoid excessive watering to prevent root rot."},
            {"Cocos nucifera", "Coconut trees are generally resilient to drizzles, but ensure good drainage to prevent waterlogging."},
            {"Ocimum tenuiflorum", "Tulsi plants can benefit from light drizzles, but avoid overwatering to prevent root rot."},
            {"Hibiscus rosa-sinensis", "Hibiscus plants usually thrive in drizzly weather conditions, but ensure good drainage to prevent waterlogging."},
            {"Bougainvillea glabra", "Bougainvillea plants can tolerate light drizzles, but ensure proper drainage to prevent waterlogging."},
            {"Jasminum sambac", "Jasmine plants can benefit from light drizzles as they help in washing away dust and pests from leaves."},
            {"Rosa indica", "Rose bushes usually thrive in drizzly weather conditions, but ensure proper drainage to prevent waterlogging."},
            {"Phyllanthus emblica", "Amla trees can tolerate light drizzles, but avoid overwatering to prevent root rot."},
            {"Ficus microcarpa", "Chinese Banyan trees usually thrive in drizzly weather conditions, but ensure proper drainage to prevent waterlogging."},
            {"Artocarpus heterophyllus", "Jackfruit trees can benefit from light drizzles as they help in washing away dust and pests from leaves."},
            {"Aegle marmelos", "Bael trees usually thrive in drizzly weather conditions, but ensure proper drainage to prevent waterlogging."},
            {"Bambusoideae", "Bamboo plants are generally resilient to drizzles, but ensure good drainage to prevent waterlogging."},
            {"Erythrina variegata", "Coral trees can tolerate light drizzles, but avoid overwatering to prevent root rot."},
            {"Murraya koenigii", "Curry leaf plants can benefit from light drizzles as they help in washing away dust and pests from leaves."},
            {"Citrus limon", "Lemon trees usually thrive in drizzly weather conditions, but ensure proper drainage to prevent waterlogging."},
            {"Psidium guajava", "Guava trees can tolerate light drizzles, but avoid overwatering to prevent root rot."},
            {"Carica papaya", "Papaya trees usually thrive in drizzly weather conditions, but ensure proper drainage to prevent waterlogging."},
            {"Tamarindus indica", "Tamarind trees can benefit from light drizzles as they help in washing away dust and pests from leaves."},
            {"Punica granatum", "Pomegranate trees usually thrive in drizzly weather conditions, but ensure proper drainage to prevent waterlogging."},
            {"Moringa oleifera", "Moringa trees can tolerate light drizzles, but avoid overwatering to prevent root rot."},
            {"Terminalia catappa", "Indian almond trees usually thrive in drizzly weather conditions, but ensure proper drainage to prevent waterlogging."},
            {"Dalbergia latifolia", "Indian rosewood trees can tolerate light drizzles, but avoid overwatering to prevent root rot."},
        };

    public Dictionary<string, string> RainTip = new Dictionary<string, string>()
        {
            {"Azadirachta indica", "Neem trees benefit from moderate rainfall, but avoid excessive watering to prevent waterlogging."},
            {"Ficus benghalensis", "Banyan trees usually thrive in rainy weather conditions, but ensure good drainage to prevent waterlogging."},
            {"Ficus religiosa", "Peepal trees generally enjoy rainfall, but ensure proper drainage to prevent waterlogging."},
            {"Mangifera indica", "Mango trees thrive in rainy weather conditions as it promotes fruit development, but avoid waterlogging to prevent root rot."},
            {"Cocos nucifera", "Coconut trees require ample water during the rainy season for proper growth and fruit development."},
            {"Ocimum tenuiflorum", "Tulsi plants benefit from regular rainfall, but ensure proper drainage to prevent waterlogging and fungal diseases."},
            {"Hibiscus rosa-sinensis", "Hibiscus plants thrive in rainy weather conditions, but avoid waterlogging to prevent root rot."},
            {"Bougainvillea glabra", "Bougainvillea plants enjoy rainfall, but ensure good drainage to prevent waterlogging and fungal diseases."},
            {"Jasminum sambac", "Jasmine plants benefit from moderate rainfall for optimal growth and flowering, but avoid waterlogging to prevent root rot."},
            {"Rosa indica", "Roses benefit from regular watering during the rainy season for lush foliage and blooming flowers."},
            {"Phyllanthus emblica", "Indian Gooseberry trees require moderate rainfall for proper growth and fruit development, but avoid waterlogging."},
            {"Ficus microcarpa", "Chinese Banyan trees thrive in rainy weather conditions, but ensure good drainage to prevent waterlogging."},
            {"Artocarpus heterophyllus", "Jackfruit trees benefit from regular watering during the rainy season for proper fruit development."},
            {"Aegle marmelos", "Bael trees require moderate rainfall for proper growth and fruit development, but avoid waterlogging."},
            {"Bambusoideae", "Bamboo plants thrive in rainy weather conditions, but ensure good drainage to prevent waterlogging."},
            {"Erythrina variegata", "Coral Trees enjoy rainfall for optimal growth and flowering, but ensure good drainage to prevent waterlogging."},
            {"Murraya koenigii", "Curry Leaf plants benefit from moderate rainfall for proper growth and foliage development."},
            {"Citrus limon", "Lemon trees require regular watering during the rainy season for proper fruit development."},
            {"Psidium guajava", "Guava trees thrive in rainy weather conditions, but ensure good drainage to prevent waterlogging."},
            {"Carica papaya", "Papaya trees require ample water during the rainy season for proper growth and fruit development."},
            {"Tamarindus indica", "Tamarind trees benefit from moderate rainfall, but ensure good drainage to prevent waterlogging."},
            {"Punica granatum", "Pomegranate trees require moderate rainfall for proper growth and fruit development, but avoid waterlogging."},
            {"Moringa oleifera", "Moringa trees benefit from regular watering during the rainy season for proper growth and leaf development."},
            {"Terminalia catappa", "Indian Almond trees thrive in rainy weather conditions, but ensure good drainage to prevent waterlogging."},
            {"Dalbergia latifolia", "Indian Rosewood trees benefit from moderate rainfall, but ensure good drainage to prevent waterlogging."},
        };

    public Dictionary<string, string> SnowTip = new Dictionary<string, string>()
        {
            {"Azadirachta indica", "Neem trees are not adapted to snowy conditions; protect them indoors or cover them with cloth during snowy weather."},
            {"Ficus benghalensis", "Banyan trees are not adapted to snowy conditions; protect them indoors during snowfall."},
            {"Ficus religiosa", "Peepal trees are not adapted to snowy conditions; protect them indoors during snowfall."},
            {"Mangifera indica", "Mango trees are not adapted to snowy conditions; protect them indoors during snowfall."},
            {"Cocos nucifera", "Coconut trees are not adapted to snowy conditions; protect them indoors during snowfall."},
            {"Ocimum tenuiflorum", "Tulsi plants are not adapted to snowy conditions; protect them indoors during snowfall."},
            {"Hibiscus rosa-sinensis", "Hibiscus plants are not adapted to snowy conditions; protect them indoors during snowfall."},
            {"Bougainvillea glabra", "Bougainvillea plants are not adapted to snowy conditions; protect them indoors during snowfall."},
            {"Jasminum sambac", "Jasmine plants are not adapted to snowy conditions; protect them indoors during snowfall."},
            {"Rosa indica", "Roses are not adapted to snowy conditions; protect them indoors during snowfall."},
            {"Phyllanthus emblica", "Indian Gooseberry trees are not adapted to snowy conditions; protect them indoors during snowfall."},
            {"Ficus microcarpa", "Chinese Banyan trees are not adapted to snowy conditions; protect them indoors during snowfall."},
            {"Artocarpus heterophyllus", "Jackfruit trees are not adapted to snowy conditions; protect them indoors during snowfall."},
            {"Aegle marmelos", "Bael trees are not adapted to snowy conditions; protect them indoors during snowfall."},
            {"Bambusoideae", "Bamboo plants are not adapted to snowy conditions; protect them indoors during snowfall."},
            {"Erythrina variegata", "Coral Trees are not adapted to snowy conditions; protect them indoors during snowfall."},
            {"Murraya koenigii", "Curry Leaf plants are not adapted to snowy conditions; protect them indoors during snowfall."},
            {"Citrus limon", "Lemon trees are not adapted to snowy conditions; protect them indoors during snowfall."},
            {"Psidium guajava", "Guava trees are not adapted to snowy conditions; protect them indoors during snowfall."},
            {"Carica papaya", "Papaya trees are not adapted to snowy conditions; protect them indoors during snowfall."},
            {"Tamarindus indica", "Tamarind trees are not adapted to snowy conditions; protect them indoors during snowfall."},
            {"Punica granatum", "Pomegranate trees are not adapted to snowy conditions; protect them indoors during snowfall."},
            {"Moringa oleifera", "Moringa trees are not adapted to snowy conditions; protect them indoors during snowfall."},
            {"Terminalia catappa", "Indian Almond trees are not adapted to snowy conditions; protect them indoors during snowfall."},
            {"Dalbergia latifolia", "Indian Rosewood trees are not adapted to snowy conditions; protect them indoors during snowfall."},
        };

    //mist
    public Dictionary<string, string> Tip701 = new Dictionary<string, string>()
        {
            {"Azadirachta indica", "Neem trees are susceptible to fungal diseases during misty conditions. Ensure good air circulation to prevent fungal growth."},
            {"Ficus benghalensis", "Banyan trees can tolerate misty conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Ficus religiosa", "Peepal trees can tolerate misty conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Mangifera indica", "Mango trees are generally tolerant of misty conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Cocos nucifera", "Coconut trees can tolerate misty conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Ocimum tenuiflorum", "Tulsi plants are susceptible to fungal diseases during misty conditions. Ensure good air circulation to prevent fungal growth."},
            {"Hibiscus rosa-sinensis", "Hibiscus plants can tolerate misty conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Bougainvillea glabra", "Bougainvillea plants are generally tolerant of misty conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Jasminum sambac", "Jasmine plants can tolerate misty conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Rosa indica", "Roses are generally tolerant of misty conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Phyllanthus emblica", "Indian Gooseberry trees are generally tolerant of misty conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Ficus microcarpa", "Chinese Banyan trees can tolerate misty conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Artocarpus heterophyllus", "Jackfruit trees can tolerate misty conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Aegle marmelos", "Bael trees are generally tolerant of misty conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Bambusoideae", "Bamboo plants are generally tolerant of misty conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Erythrina variegata", "Coral Trees can tolerate misty conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Murraya koenigii", "Curry Leaf plants can tolerate misty conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Citrus limon", "Lemon trees can tolerate misty conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Psidium guajava", "Guava trees are generally tolerant of misty conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Carica papaya", "Papaya trees are generally tolerant of misty conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Tamarindus indica", "Tamarind trees can tolerate misty conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Punica granatum", "Pomegranate trees are generally tolerant of misty conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Moringa oleifera", "Moringa trees can tolerate misty conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Terminalia catappa", "Indian Almond trees can tolerate misty conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Dalbergia latifolia", "Indian Rosewood trees can tolerate misty conditions, but ensure proper air circulation to prevent fungal diseases."},
        };

    //smoke
    public Dictionary<string, string> Tip711 = new Dictionary<string, string>()
        {
            {"Azadirachta indica", "Neem trees can be affected by smoke, leading to reduced photosynthesis. Protect them from exposure to heavy smoke."},
            {"Ficus benghalensis", "Banyan trees can be affected by smoke, leading to reduced photosynthesis. Protect them from exposure to heavy smoke."},
            {"Ficus religiosa", "Peepal trees can be affected by smoke, leading to reduced photosynthesis. Protect them from exposure to heavy smoke."},
            {"Mangifera indica", "Mango trees can be affected by smoke, leading to reduced photosynthesis. Protect them from exposure to heavy smoke."},
            {"Cocos nucifera", "Coconut trees can be affected by smoke, leading to reduced photosynthesis. Protect them from exposure to heavy smoke."},
            {"Ocimum tenuiflorum", "Tulsi plants can be affected by smoke, leading to reduced photosynthesis. Protect them from exposure to heavy smoke."},
            {"Hibiscus rosa-sinensis", "Hibiscus plants can be affected by smoke, leading to reduced photosynthesis. Protect them from exposure to heavy smoke."},
            {"Bougainvillea glabra", "Bougainvillea plants can be affected by smoke, leading to reduced photosynthesis. Protect them from exposure to heavy smoke."},
            {"Jasminum sambac", "Jasmine plants can be affected by smoke, leading to reduced photosynthesis. Protect them from exposure to heavy smoke."},
            {"Rosa indica", "Roses can be affected by smoke, leading to reduced photosynthesis. Protect them from exposure to heavy smoke."},
            {"Phyllanthus emblica", "Indian Gooseberry trees can be affected by smoke, leading to reduced photosynthesis. Protect them from exposure to heavy smoke."},
            {"Ficus microcarpa", "Chinese Banyan trees can be affected by smoke, leading to reduced photosynthesis. Protect them from exposure to heavy smoke."},
            {"Artocarpus heterophyllus", "Jackfruit trees can be affected by smoke, leading to reduced photosynthesis. Protect them from exposure to heavy smoke."},
            {"Aegle marmelos", "Bael trees can be affected by smoke, leading to reduced photosynthesis. Protect them from exposure to heavy smoke."},
            {"Bambusoideae", "Bamboo plants can be affected by smoke, leading to reduced photosynthesis. Protect them from exposure to heavy smoke."},
            {"Erythrina variegata", "Coral Trees can be affected by smoke, leading to reduced photosynthesis. Protect them from exposure to heavy smoke."},
            {"Murraya koenigii", "Curry Leaf plants can be affected by smoke, leading to reduced photosynthesis. Protect them from exposure to heavy smoke."},
            {"Citrus limon", "Lemon trees can be affected by smoke, leading to reduced photosynthesis. Protect them from exposure to heavy smoke."},
            {"Psidium guajava", "Guava trees can be affected by smoke, leading to reduced photosynthesis. Protect them from exposure to heavy smoke."},
            {"Carica papaya", "Papaya trees can be affected by smoke, leading to reduced photosynthesis. Protect them from exposure to heavy smoke."},
            {"Tamarindus indica", "Tamarind trees can be affected by smoke, leading to reduced photosynthesis. Protect them from exposure to heavy smoke."},
            {"Punica granatum", "Pomegranate trees can be affected by smoke, leading to reduced photosynthesis. Protect them from exposure to heavy smoke."},
            {"Moringa oleifera", "Moringa trees can be affected by smoke, leading to reduced photosynthesis. Protect them from exposure to heavy smoke."},
            {"Terminalia catappa", "Indian Almond trees can be affected by smoke, leading to reduced photosynthesis. Protect them from exposure to heavy smoke."},
            {"Dalbergia latifolia", "Indian Rosewood trees can be affected by smoke, leading to reduced photosynthesis. Protect them from exposure to heavy smoke."},
        };

    //haze
    public Dictionary<string, string> Tip721 = new Dictionary<string, string>()
        {
            {"Azadirachta indica", "Neem trees can tolerate haze conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Ficus benghalensis", "Banyan trees can tolerate haze conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Ficus religiosa", "Peepal trees can tolerate haze conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Mangifera indica", "Mango trees can tolerate haze conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Cocos nucifera", "Coconut trees can tolerate haze conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Ocimum tenuiflorum", "Tulsi plants can tolerate haze conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Hibiscus rosa-sinensis", "Hibiscus plants can tolerate haze conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Bougainvillea glabra", "Bougainvillea plants can tolerate haze conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Jasminum sambac", "Jasmine plants can tolerate haze conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Rosa indica", "Roses can tolerate haze conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Phyllanthus emblica", "Indian Gooseberry trees can tolerate haze conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Ficus microcarpa", "Chinese Banyan trees can tolerate haze conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Artocarpus heterophyllus", "Jackfruit trees can tolerate haze conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Aegle marmelos", "Bael trees can tolerate haze conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Bambusoideae", "Bamboo plants can tolerate haze conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Erythrina variegata", "Coral Trees can tolerate haze conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Murraya koenigii", "Curry Leaf plants can tolerate haze conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Citrus limon", "Lemon trees can tolerate haze conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Psidium guajava", "Guava trees can tolerate haze conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Carica papaya", "Papaya trees can tolerate haze conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Tamarindus indica", "Tamarind trees can tolerate haze conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Punica granatum", "Pomegranate trees can tolerate haze conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Moringa oleifera", "Moringa trees can tolerate haze conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Terminalia catappa", "Indian Almond trees can tolerate haze conditions, but ensure proper air circulation to prevent fungal diseases."},
            {"Dalbergia latifolia", "Indian Rosewood trees can tolerate haze conditions, but ensure proper air circulation to prevent fungal diseases."},
        };

    //dust
    public Dictionary<string, string> Tip731 = new Dictionary<string, string>()
        {
            {"Azadirachta indica", "Neem trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust."},
            {"Ficus benghalensis", "Banyan trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust."},
            {"Ficus religiosa", "Peepal trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust."},
            {"Mangifera indica", "Mango trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust."},
            {"Cocos nucifera", "Coconut trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust."},
            {"Ocimum tenuiflorum", "Tulsi plants can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust."},
            {"Hibiscus rosa-sinensis", "Hibiscus plants can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust."},
            {"Bougainvillea glabra", "Bougainvillea plants can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust."},
            {"Jasminum sambac", "Jasmine plants can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust."},
            {"Rosa indica", "Roses can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust."},
            {"Phyllanthus emblica", "Indian Gooseberry trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust."},
            {"Ficus microcarpa", "Chinese Banyan trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust."},
            {"Artocarpus heterophyllus", "Jackfruit trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust."},
            {"Aegle marmelos", "Bael trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust."},
            {"Bambusoideae", "Bamboo plants can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust."},
            {"Erythrina variegata", "Coral Trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust."},
            {"Murraya koenigii", "Curry Leaf plants can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust."},
            {"Citrus limon", "Lemon trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust."},
            {"Psidium guajava", "Guava trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust."},
            {"Carica papaya", "Papaya trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust."},
            {"Tamarindus indica", "Tamarind trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust."},
            {"Punica granatum", "Pomegranate trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust."},
            {"Moringa oleifera", "Moringa trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust."},
            {"Terminalia catappa", "Indian Almond trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust."},
            {"Dalbergia latifolia", "Indian Rosewood trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust."},
        };

    //fog
    public Dictionary<string, string> Tip741 = new Dictionary<string, string>()
        {
            {"Azadirachta indica", "Neem trees can tolerate foggy conditions, but ensure good air circulation to prevent fungal diseases."},
            {"Ficus benghalensis", "Banyan trees can tolerate foggy conditions, but ensure good air circulation to prevent fungal diseases."},
            {"Ficus religiosa", "Peepal trees can tolerate foggy conditions, but ensure good air circulation to prevent fungal diseases."},
            {"Mangifera indica", "Mango trees can tolerate foggy conditions, but ensure good air circulation to prevent fungal diseases."},
            {"Cocos nucifera", "Coconut trees can tolerate foggy conditions, but ensure good air circulation to prevent fungal diseases."},
            {"Ocimum tenuiflorum", "Tulsi plants can tolerate foggy conditions, but ensure good air circulation to prevent fungal diseases."},
            {"Hibiscus rosa-sinensis", "Hibiscus plants can tolerate foggy conditions, but ensure good air circulation to prevent fungal diseases."},
            {"Bougainvillea glabra", "Bougainvillea plants can tolerate foggy conditions, but ensure good air circulation to prevent fungal diseases."},
            {"Jasminum sambac", "Jasmine plants can tolerate foggy conditions, but ensure good air circulation to prevent fungal diseases."},
            {"Rosa indica", "Roses can tolerate foggy conditions, but ensure good air circulation to prevent fungal diseases."},
            {"Phyllanthus emblica", "Indian Gooseberry trees can tolerate foggy conditions, but ensure good air circulation to prevent fungal diseases."},
            {"Ficus microcarpa", "Chinese Banyan trees can tolerate foggy conditions, but ensure good air circulation to prevent fungal diseases."},
            {"Artocarpus heterophyllus", "Jackfruit trees can tolerate foggy conditions, but ensure good air circulation to prevent fungal diseases."},
            {"Aegle marmelos", "Bael trees can tolerate foggy conditions, but ensure good air circulation to prevent fungal diseases."},
            {"Bambusoideae", "Bamboo plants can tolerate foggy conditions, but ensure good air circulation to prevent fungal diseases."},
            {"Erythrina variegata", "Coral Trees can tolerate foggy conditions, but ensure good air circulation to prevent fungal diseases."},
            {"Murraya koenigii", "Curry Leaf plants can tolerate foggy conditions, but ensure good air circulation to prevent fungal diseases."},
            {"Citrus limon", "Lemon trees can tolerate foggy conditions, but ensure good air circulation to prevent fungal diseases."},
            {"Psidium guajava", "Guava trees can tolerate foggy conditions, but ensure good air circulation to prevent fungal diseases."},
            {"Carica papaya", "Papaya trees can tolerate foggy conditions, but ensure good air circulation to prevent fungal diseases."},
            {"Tamarindus indica", "Tamarind trees can tolerate foggy conditions, but ensure good air circulation to prevent fungal diseases."},
            {"Punica granatum", "Pomegranate trees can tolerate foggy conditions, but ensure good air circulation to prevent fungal diseases."},
            {"Moringa oleifera", "Moringa trees can tolerate foggy conditions, but ensure good air circulation to prevent fungal diseases."},
            {"Terminalia catappa", "Indian Almond trees can tolerate foggy conditions, but ensure good air circulation to prevent fungal diseases."},
            {"Dalbergia latifolia", "Indian Rosewood trees can tolerate foggy conditions, but ensure good air circulation to prevent fungal diseases."},
        };

    //sand
    public Dictionary<string, string> Tip751 = new Dictionary<string, string>()
        {
            {"Azadirachta indica", "Neem trees can tolerate sandy conditions, but ensure proper irrigation to maintain soil moisture."},
            {"Ficus benghalensis", "Banyan trees can tolerate sandy conditions, but ensure proper irrigation to maintain soil moisture."},
            {"Ficus religiosa", "Peepal trees can tolerate sandy conditions, but ensure proper irrigation to maintain soil moisture."},
            {"Mangifera indica", "Mango trees can tolerate sandy conditions, but ensure proper irrigation to maintain soil moisture."},
            {"Cocos nucifera", "Coconut trees can tolerate sandy conditions, but ensure proper irrigation to maintain soil moisture."},
            {"Ocimum tenuiflorum", "Tulsi plants can tolerate sandy conditions, but ensure proper irrigation to maintain soil moisture."},
            {"Hibiscus rosa-sinensis", "Hibiscus plants can tolerate sandy conditions, but ensure proper irrigation to maintain soil moisture."},
            {"Bougainvillea glabra", "Bougainvillea plants can tolerate sandy conditions, but ensure proper irrigation to maintain soil moisture."},
            {"Jasminum sambac", "Jasmine plants can tolerate sandy conditions, but ensure proper irrigation to maintain soil moisture."},
            {"Rosa indica", "Roses can tolerate sandy conditions, but ensure proper irrigation to maintain soil moisture."},
            {"Phyllanthus emblica", "Indian Gooseberry trees can tolerate sandy conditions, but ensure proper irrigation to maintain soil moisture."},
            {"Ficus microcarpa", "Chinese Banyan trees can tolerate sandy conditions, but ensure proper irrigation to maintain soil moisture."},
            {"Artocarpus heterophyllus", "Jackfruit trees can tolerate sandy conditions, but ensure proper irrigation to maintain soil moisture."},
            {"Aegle marmelos", "Bael trees can tolerate sandy conditions, but ensure proper irrigation to maintain soil moisture."},
            {"Bambusoideae", "Bamboo plants can tolerate sandy conditions, but ensure proper irrigation to maintain soil moisture."},
            {"Erythrina variegata", "Coral Trees can tolerate sandy conditions, but ensure proper irrigation to maintain soil moisture."},
            {"Murraya koenigii", "Curry Leaf plants can tolerate sandy conditions, but ensure proper irrigation to maintain soil moisture."},
            {"Citrus limon", "Lemon trees can tolerate sandy conditions, but ensure proper irrigation to maintain soil moisture."},
            {"Psidium guajava", "Guava trees can tolerate sandy conditions, but ensure proper irrigation to maintain soil moisture."},
            {"Carica papaya", "Papaya trees can tolerate sandy conditions, but ensure proper irrigation to maintain soil moisture."},
            {"Tamarindus indica", "Tamarind trees can tolerate sandy conditions, but ensure proper irrigation to maintain soil moisture."},
            {"Punica granatum", "Pomegranate trees can tolerate sandy conditions, but ensure proper irrigation to maintain soil moisture."},
            {"Moringa oleifera", "Moringa trees can tolerate sandy conditions, but ensure proper irrigation to maintain soil moisture."},
            {"Terminalia catappa", "Indian Almond trees can tolerate sandy conditions, but ensure proper irrigation to maintain soil moisture."},
            {"Dalbergia latifolia", "Indian Rosewood trees can tolerate sandy conditions, but ensure proper irrigation to maintain soil moisture."},
        };

    //dust
    public Dictionary<string, string> Tip761 = new Dictionary<string, string>()
        {
            {"Azadirachta indica", "Neem trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust and ensure proper air circulation around the plant."},
            {"Ficus benghalensis", "Banyan trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust and ensure proper air circulation around the plant."},
            {"Ficus religiosa", "Peepal trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust and ensure proper air circulation around the plant."},
            {"Mangifera indica", "Mango trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust and ensure proper air circulation around the plant."},
            {"Cocos nucifera", "Coconut trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust and ensure proper air circulation around the plant."},
            {"Ocimum tenuiflorum", "Tulsi plants can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust and ensure proper air circulation around the plant."},
            {"Hibiscus rosa-sinensis", "Hibiscus plants can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust and ensure proper air circulation around the plant."},
            {"Bougainvillea glabra", "Bougainvillea plants can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust and ensure proper air circulation around the plant."},
            {"Jasminum sambac", "Jasmine plants can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust and ensure proper air circulation around the plant."},
            {"Rosa indica", "Roses can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust and ensure proper air circulation around the plant."},
            {"Phyllanthus emblica", "Indian Gooseberry trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust and ensure proper air circulation around the plant."},
            {"Ficus microcarpa", "Chinese Banyan trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust and ensure proper air circulation around the plant."},
            {"Artocarpus heterophyllus", "Jackfruit trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust and ensure proper air circulation around the plant."},
            {"Aegle marmelos", "Bael trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust and ensure proper air circulation around the plant."},
            {"Bambusoideae", "Bamboo plants can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust and ensure proper air circulation around the plant."},
            {"Erythrina variegata", "Coral Trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust and ensure proper air circulation around the plant."},
            {"Murraya koenigii", "Curry Leaf plants can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust and ensure proper air circulation around the plant."},
            {"Citrus limon", "Lemon trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust and ensure proper air circulation around the plant."},
            {"Psidium guajava", "Guava trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust and ensure proper air circulation around the plant."},
            {"Carica papaya", "Papaya trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust and ensure proper air circulation around the plant."},
            {"Tamarindus indica", "Tamarind trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust and ensure proper air circulation around the plant."},
            {"Punica granatum", "Pomegranate trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust and ensure proper air circulation around the plant."},
            {"Moringa oleifera", "Moringa trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust and ensure proper air circulation around the plant."},
            {"Terminalia catappa", "Indian Almond trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust and ensure proper air circulation around the plant."},
            {"Dalbergia latifolia", "Indian Rosewood trees can tolerate dusty conditions, but gently clean their leaves to remove accumulated dust and ensure proper air circulation around the plant."},
        };

    //squall
    public Dictionary<string, string> Tip771 = new Dictionary<string, string>()
        {
            {"Azadirachta indica", "Neem trees can tolerate squall conditions, but ensure sturdy support for young plants and secure loose branches to prevent damage."},
            {"Ficus benghalensis", "Banyan trees can tolerate squall conditions, but ensure sturdy support for young plants and secure loose branches to prevent damage."},
            {"Ficus religiosa", "Peepal trees can tolerate squall conditions, but ensure sturdy support for young plants and secure loose branches to prevent damage."},
            {"Mangifera indica", "Mango trees can tolerate squall conditions, but ensure sturdy support for young plants and secure loose branches to prevent damage."},
            {"Cocos nucifera", "Coconut trees can tolerate squall conditions, but ensure sturdy support for young plants and secure loose branches to prevent damage."},
            {"Ocimum tenuiflorum", "Tulsi plants can tolerate squall conditions, but ensure sturdy support for young plants and secure loose branches to prevent damage."},
            {"Hibiscus rosa-sinensis", "Hibiscus plants can tolerate squall conditions, but ensure sturdy support for young plants and secure loose branches to prevent damage."},
            {"Bougainvillea glabra", "Bougainvillea plants can tolerate squall conditions, but ensure sturdy support for young plants and secure loose branches to prevent damage."},
            {"Jasminum sambac", "Jasmine plants can tolerate squall conditions, but ensure sturdy support for young plants and secure loose branches to prevent damage."},
            {"Rosa indica", "Roses can tolerate squall conditions, but ensure sturdy support for young plants and secure loose branches to prevent damage."},
            {"Phyllanthus emblica", "Indian Gooseberry trees can tolerate squall conditions, but ensure sturdy support for young plants and secure loose branches to prevent damage."},
            {"Ficus microcarpa", "Chinese Banyan trees can tolerate squall conditions, but ensure sturdy support for young plants and secure loose branches to prevent damage."},
            {"Artocarpus heterophyllus", "Jackfruit trees can tolerate squall conditions, but ensure sturdy support for young plants and secure loose branches to prevent damage."},
            {"Aegle marmelos", "Bael trees can tolerate squall conditions, but ensure sturdy support for young plants and secure loose branches to prevent damage."},
            {"Bambusoideae", "Bamboo plants can tolerate squall conditions, but ensure sturdy support for young plants and secure loose branches to prevent damage."},
            {"Erythrina variegata", "Coral Trees can tolerate squall conditions, but ensure sturdy support for young plants and secure loose branches to prevent damage."},
            {"Murraya koenigii", "Curry Leaf plants can tolerate squall conditions, but ensure sturdy support for young plants and secure loose branches to prevent damage."},
            {"Citrus limon", "Lemon trees can tolerate squall conditions, but ensure sturdy support for young plants and secure loose branches to prevent damage."},
            {"Psidium guajava", "Guava trees can tolerate squall conditions, but ensure sturdy support for young plants and secure loose branches to prevent damage."},
            {"Carica papaya", "Papaya trees can tolerate squall conditions, but ensure sturdy support for young plants and secure loose branches to prevent damage."},
            {"Tamarindus indica", "Tamarind trees can tolerate squall conditions, but ensure sturdy support for young plants and secure loose branches to prevent damage."},
            {"Punica granatum", "Pomegranate trees can tolerate squall conditions, but ensure sturdy support for young plants and secure loose branches to prevent damage."},
            {"Moringa oleifera", "Moringa trees can tolerate squall conditions, but ensure sturdy support for young plants and secure loose branches to prevent damage."},
            {"Terminalia catappa", "Indian Almond trees can tolerate squall conditions, but ensure sturdy support for young plants and secure loose branches to prevent damage."},
            {"Dalbergia latifolia", "Indian Rosewood trees can tolerate squall conditions, but ensure sturdy support for young plants and secure loose branches to prevent damage."},
        };

    public Dictionary<string, string> ClearTip = new Dictionary<string, string>()
        {
            {"Azadirachta indica", "Neem trees thrive under clear sky conditions, ensuring ample sunlight exposure for healthy growth and development."},
            {"Ficus benghalensis", "Banyan trees thrive under clear sky conditions, ensuring ample sunlight exposure for healthy growth and development."},
            {"Ficus religiosa", "Peepal trees thrive under clear sky conditions, ensuring ample sunlight exposure for healthy growth and development."},
            {"Mangifera indica", "Mango trees thrive under clear sky conditions, ensuring ample sunlight exposure for healthy growth and development."},
            {"Cocos nucifera", "Coconut trees thrive under clear sky conditions, ensuring ample sunlight exposure for healthy growth and development."},
            {"Ocimum tenuiflorum", "Tulsi plants thrive under clear sky conditions, ensuring ample sunlight exposure for healthy growth and development."},
            {"Hibiscus rosa-sinensis", "Hibiscus plants thrive under clear sky conditions, ensuring ample sunlight exposure for healthy growth and development."},
            {"Bougainvillea glabra", "Bougainvillea plants thrive under clear sky conditions, ensuring ample sunlight exposure for healthy growth and development."},
            {"Jasminum sambac", "Jasmine plants thrive under clear sky conditions, ensuring ample sunlight exposure for healthy growth and development."},
            {"Rosa indica", "Roses thrive under clear sky conditions, ensuring ample sunlight exposure for healthy growth and development."},
            {"Phyllanthus emblica", "Indian Gooseberry trees thrive under clear sky conditions, ensuring ample sunlight exposure for healthy growth and development."},
            {"Ficus microcarpa", "Chinese Banyan trees thrive under clear sky conditions, ensuring ample sunlight exposure for healthy growth and development."},
            {"Artocarpus heterophyllus", "Jackfruit trees thrive under clear sky conditions, ensuring ample sunlight exposure for healthy growth and development."},
            {"Aegle marmelos", "Bael trees thrive under clear sky conditions, ensuring ample sunlight exposure for healthy growth and development."},
            {"Bambusoideae", "Bamboo plants thrive under clear sky conditions, ensuring ample sunlight exposure for healthy growth and development."},
            {"Erythrina variegata", "Coral Trees thrive under clear sky conditions, ensuring ample sunlight exposure for healthy growth and development."},
            {"Murraya koenigii", "Curry Leaf plants thrive under clear sky conditions, ensuring ample sunlight exposure for healthy growth and development."},
            {"Citrus limon", "Lemon trees thrive under clear sky conditions, ensuring ample sunlight exposure for healthy growth and development."},
            {"Psidium guajava", "Guava trees thrive under clear sky conditions, ensuring ample sunlight exposure for healthy growth and development."},
            {"Carica papaya", "Papaya trees thrive under clear sky conditions, ensuring ample sunlight exposure for healthy growth and development."},
            {"Tamarindus indica", "Tamarind trees thrive under clear sky conditions, ensuring ample sunlight exposure for healthy growth and development."},
            {"Punica granatum", "Pomegranate trees thrive under clear sky conditions, ensuring ample sunlight exposure for healthy growth and development."},
            {"Moringa oleifera", "Moringa trees thrive under clear sky conditions, ensuring ample sunlight exposure for healthy growth and development."},
            {"Terminalia catappa", "Indian Almond trees thrive under clear sky conditions, ensuring ample sunlight exposure for healthy growth and development."},
            {"Dalbergia latifolia", "Indian Rosewood trees thrive under clear sky conditions, ensuring ample sunlight exposure for healthy growth and development."},
        };

    public Dictionary<string, string> CloudyTip = new Dictionary<string, string>()
        {
            {"Azadirachta indica", "Neem trees can still thrive under cloudy sky conditions, but may experience slower growth rates due to reduced sunlight. Monitor soil moisture levels carefully to ensure proper hydration during periods of decreased sunlight."},
            {"Ficus benghalensis", "Banyan trees can still thrive under cloudy sky conditions, but may experience slower growth rates due to reduced sunlight. Monitor soil moisture levels carefully to ensure proper hydration during periods of decreased sunlight."},
            {"Ficus religiosa", "Peepal trees can still thrive under cloudy sky conditions, but may experience slower growth rates due to reduced sunlight. Monitor soil moisture levels carefully to ensure proper hydration during periods of decreased sunlight."},
            {"Mangifera indica", "Mango trees can still thrive under cloudy sky conditions, but may experience slower growth rates due to reduced sunlight. Monitor soil moisture levels carefully to ensure proper hydration during periods of decreased sunlight."},
            {"Cocos nucifera", "Coconut trees can still thrive under cloudy sky conditions, but may experience slower growth rates due to reduced sunlight. Monitor soil moisture levels carefully to ensure proper hydration during periods of decreased sunlight."},
            {"Ocimum tenuiflorum", "Tulsi plants can still thrive under cloudy sky conditions, but may experience slower growth rates due to reduced sunlight. Monitor soil moisture levels carefully to ensure proper hydration during periods of decreased sunlight."},
            {"Hibiscus rosa-sinensis", "Hibiscus plants can still thrive under cloudy sky conditions, but may experience slower growth rates due to reduced sunlight. Monitor soil moisture levels carefully to ensure proper hydration during periods of decreased sunlight."},
            {"Bougainvillea glabra", "Bougainvillea plants can still thrive under cloudy sky conditions, but may experience slower growth rates due to reduced sunlight. Monitor soil moisture levels carefully to ensure proper hydration during periods of decreased sunlight."},
            {"Jasminum sambac", "Jasmine plants can still thrive under cloudy sky conditions, but may experience slower growth rates due to reduced sunlight. Monitor soil moisture levels carefully to ensure proper hydration during periods of decreased sunlight."},
            {"Rosa indica", "Roses can still thrive under cloudy sky conditions, but may experience slower growth rates due to reduced sunlight. Monitor soil moisture levels carefully to ensure proper hydration during periods of decreased sunlight."},
            {"Phyllanthus emblica", "Indian Gooseberry trees can still thrive under cloudy sky conditions, but may experience slower growth rates due to reduced sunlight. Monitor soil moisture levels carefully to ensure proper hydration during periods of decreased sunlight."},
            {"Ficus microcarpa", "Chinese Banyan trees can still thrive under cloudy sky conditions, but may experience slower growth rates due to reduced sunlight. Monitor soil moisture levels carefully to ensure proper hydration during periods of decreased sunlight."},
            {"Artocarpus heterophyllus", "Jackfruit trees can still thrive under cloudy sky conditions, but may experience slower growth rates due to reduced sunlight. Monitor soil moisture levels carefully to ensure proper hydration during periods of decreased sunlight."},
            {"Aegle marmelos", "Bael trees can still thrive under cloudy sky conditions, but may experience slower growth rates due to reduced sunlight. Monitor soil moisture levels carefully to ensure proper hydration during periods of decreased sunlight."},
            {"Bambusoideae", "Bamboo plants can still thrive under cloudy sky conditions, but may experience slower growth rates due to reduced sunlight. Monitor soil moisture levels carefully to ensure proper hydration during periods of decreased sunlight."},
            {"Erythrina variegata", "Coral Trees can still thrive under cloudy sky conditions, but may experience slower growth rates due to reduced sunlight. Monitor soil moisture levels carefully to ensure proper hydration during periods of decreased sunlight."},
            {"Murraya koenigii", "Curry Leaf plants can still thrive under cloudy sky conditions, but may experience slower growth rates due to reduced sunlight. Monitor soil moisture levels carefully to ensure proper hydration during periods of decreased sunlight."},
            {"Citrus limon", "Lemon trees can still thrive under cloudy sky conditions, but may experience slower growth rates due to reduced sunlight. Monitor soil moisture levels carefully to ensure proper hydration during periods of decreased sunlight."},
            {"Psidium guajava", "Guava trees can still thrive under cloudy sky conditions, but may experience slower growth rates due to reduced sunlight. Monitor soil moisture levels carefully to ensure proper hydration during periods of decreased sunlight."},
            {"Carica papaya", "Papaya trees can still thrive under cloudy sky conditions, but may experience slower growth rates due to reduced sunlight. Monitor soil moisture levels carefully to ensure proper hydration during periods of decreased sunlight."},
            {"Tamarindus indica", "Tamarind trees can still thrive under cloudy sky conditions, but may experience slower growth rates due to reduced sunlight. Monitor soil moisture levels carefully to ensure proper hydration during periods of decreased sunlight."},
            {"Punica granatum", "Pomegranate trees can still thrive under cloudy sky conditions, but may experience slower growth rates due to reduced sunlight. Monitor soil moisture levels carefully to ensure proper hydration during periods of decreased sunlight."},
            {"Moringa oleifera", "Moringa trees can still thrive under cloudy sky conditions, but may experience slower growth rates due to reduced sunlight. Monitor soil moisture levels carefully to ensure proper hydration during periods of decreased sunlight."},
            {"Terminalia catappa", "Indian Almond trees can still thrive under cloudy sky conditions, but may experience slower growth rates due to reduced sunlight. Monitor soil moisture levels carefully to ensure proper hydration during periods of decreased sunlight."},
            {"Dalbergia latifolia", "Indian Rosewood trees can still thrive under cloudy sky conditions, but may experience slower growth rates due to reduced sunlight. Monitor soil moisture levels carefully to ensure proper hydration during periods of decreased sunlight."},
        };

    public void UpdatePlantInfo()
    {
        Debug.Log("update plant info");
        //plantInfo.text = Info["Hibiscus rosa-sinensis"];
        plantInfo.text = Info[plantNetAPI.scientificName];

        weatherCode = weatherData.Info.weather[0].id;
        switch (weatherCode)
        {
            case 200:
            case 201:
            case 202:
            case 210:
            case 211:
            case 212:
            case 221:
            case 230:
            case 231:
            case 232:
                plantTip.text = ThunderStormTip[plantNetAPI.scientificName];
                break;

            case 300:
            case 301:
            case 302:
            case 310:
            case 311:
            case 312:
            case 313:
            case 314:
            case 321:
                plantTip.text = DrizzleTip[plantNetAPI.scientificName];
                break;

            case 500:
            case 501:
            case 502:
            case 503:
            case 504:
            case 511:
            case 520:
            case 521:
            case 522:
            case 531:
                plantTip.text = RainTip[plantNetAPI.scientificName];
                break;

            case 600:
            case 601:
            case 602:
            case 611:
            case 612:
            case 613:
            case 615:
            case 616:
            case 620:
            case 621:
            case 622:
                plantTip.text = SnowTip[plantNetAPI.scientificName];
                break;

            case 701:
                plantTip.text = Tip701[plantNetAPI.scientificName];
                break;

            case 711:
                plantTip.text = Tip711[plantNetAPI.scientificName];
                break;

            case 721:
                plantTip.text = Tip721[plantNetAPI.scientificName];
                break;

            case 731:
                plantTip.text = Tip731[plantNetAPI.scientificName];
                break;

            case 741:
                plantTip.text = Tip741[plantNetAPI.scientificName];
                break;

            case 751:
                plantTip.text = Tip751[plantNetAPI.scientificName];
                break;

            case 761:
                plantTip.text = Tip761[plantNetAPI.scientificName];
                break;

            case 771:
                plantTip.text = Tip771[plantNetAPI.scientificName];
                break;

            case 800:
                plantTip.text = ClearTip[plantNetAPI.scientificName];
                break;

            case 801:
            case 802:
            case 803:
            case 804:
                plantTip.text = CloudyTip[plantNetAPI.scientificName];
                break;
        }
    }

    private void Start()
    {
        weatherInfo = weatherData.Info;
    }
}
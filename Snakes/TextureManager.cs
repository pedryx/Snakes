using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

namespace Snakes_Client
{
    /// <summary>
    /// Represent a global manager of textures.
    /// (Singleton)
    /// </summary>
    public class TextureManager
    {

        #region Singleton
        /// <summary>
        /// Represent the only instance of texture manager.
        /// </summary>
        private static TextureManager instance;

        /// <summary>
        /// Get instance of texture manager.
        /// </summary>
        public static TextureManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new TextureManager();

                return instance;
            }
        }
        #endregion

        /// <summary>
        /// Folder that contains all textures.
        /// </summary>
        private const String texturesFolder = "Textures";

        /// <summary>
        /// Contains all loaded texture.
        /// </summary>
        private readonly Dictionary<String, Dictionary<Color, Texture>> textures;

        /// <summary>
        /// Create new texture manager.
        /// </summary>
        private TextureManager()
        {
            textures = new Dictionary<String, Dictionary<Color, Texture>>();
        }

        /// <summary>
        /// Get texture with specific name, colorized by speicifc color.
        /// </summary>
        /// <param name="name">Name of the texture to get.</param>
        /// <param name="color">Color that will be applied to texture.</param>
        /// <returns>Texture with specific name, colorized by specific color.</returns>
        public Texture GetTexture(String name, Color color)
        {
            if (!textures.ContainsKey(name) || !textures[name].ContainsKey(color))
                LoadTexture(name, color);

            return textures[name][color];
        }

        /// <summary>
        /// Get texture with specific name.
        /// </summary>
        /// <param name="name">Name of the texture to get.</param>
        /// <returns>Texture with specific name.</returns>
        public Texture GetTexture(String name)
            => GetTexture(name, Colors.White);

        /// <summary>
        /// Load texture with specific name and colorize it by specific color.
        /// The texture will be saved to cache.
        /// </summary>
        /// <param name="name">Name of the texture to load.</param>
        /// <param name="color">Color that will be applied to texture.r</param>
        public void LoadTexture(String name, Color color)
        {
            Texture texture = new Texture(Path.Combine(texturesFolder, $"{name}.png"), color);
            if (!textures.ContainsKey(name))
                textures.Add(name, new Dictionary<Color, Texture>());
            if (!textures[name].ContainsKey(color))
                textures[name].Add(color, texture);
        }

        /// <summary>
        /// Load texture with specific name.
        /// The texture will be saved to cache.
        /// </summary>
        /// <param name="name">Name of the texture to load.</param>
        public void LoadTexture(String name)
            => LoadTexture(name, Colors.White);

        /// <summary>
        /// Load textures and colorize them.
        /// The textures will be saved to cache.
        /// </summary>
        /// <param name="color">Color that will be aplied to textures/</param>
        /// <param name="names">Names of textures that will be loaded.</param>
        public void LoadTextures(Color color, params String[] names)
        {
            foreach (var name in names)
                LoadTexture(name, color);
        }

        /// <summary>
        /// Load textures.
        /// The textures will be saved to cache.
        /// </summary>
        /// <param name="names">Names of textures that will be loaded.</param>
        public void LoadTextures(params String[] names)
            => LoadTextures(Colors.White, names);

        /// <summary>
        /// Get texture with specific name.
        /// </summary>
        /// <param name="name">Name of the texture to get.</param>
        /// <returns>Texture with specific name.</returns>
        public Texture this[String name]
        {
            get => GetTexture(name);
        }

    }
}

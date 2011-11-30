using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileEngine;
using System.IO;

namespace TileEditor
{
    using Image = System.Drawing.Image;
    using Point = System.Drawing.Point;

    public partial class Form1 : Form
    {
        SpriteBatch spriteBatch;

        Texture2D tileTexture;

        string[] imageExtentions = new string[]
        {
            ".jpg", ".png", ".tga"
        };

        int maxWidth = 0, maxHeight = 0;

        Camera _camera = new Camera();
        TileMap tileMap = new TileMap();

        Dictionary<string, TileLayer> tileLayerDictionary = new Dictionary<string, TileLayer>();
        Dictionary<string, Texture2D> textureDictionary = new Dictionary<string, Texture2D>();
        Dictionary<string, Image> previewDictionary = new Dictionary<string, Image>();

        TileLayer selectedLayer = null;
        int cellX = 0, cellY = 0;
        private bool closeImmediately = false;

        public GraphicsDevice GraphicsDevice
        {
            get { return tileDisplay1.GraphicsDevice; }
        }

        public Form1()
        {
            InitializeComponent();

            Application.Idle += delegate { tileDisplay1.Invalidate(); };
            tileDisplay1.BackColor = System.Drawing.Color.Black;

            Mouse.WindowHandle = tileDisplay1.Handle;

            while (string.IsNullOrEmpty(contentPathTextbox.Text))
            {
                if (folderBrowserDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    contentPathTextbox.Text = folderBrowserDialog1.SelectedPath;
                }

                if (string.IsNullOrEmpty(contentPathTextbox.Text))
                {
                    if (MessageBox.Show("Please select a content directory.", "Content Directory Required", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        closeImmediately = true;
                        return;
                    }
                }
            }

            AdjustScrollbars(tileMap);

        }

        private void newTileMapToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Layers (*.layer)|*.layer";
            if (openFileDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                string filename = openFileDialog1.FileName;

                string[] textureNames;
                TileLayer layer = TileLayer.FromFile(filename, out textureNames);

                string key = Path.GetFileNameWithoutExtension(filename);

                if (tileLayerDictionary.ContainsKey(key))
                {
                    tileMap.Layers.Remove(tileLayerDictionary[key]);
                    tileLayerDictionary.Remove(key);
                    layerListBox.Items.Remove(key);
                }

                tileLayerDictionary.Add(key, layer);
                tileMap.Layers.Add(layer);
                layerListBox.Items.Add(key);

                foreach (string textureName in textureNames)
                {
                    if (textureDictionary.ContainsKey(textureName))
                    {
                        layer.AddTexture(textureDictionary[textureName]);
                    }
                    else
                    {
                        string fullPath = Path.Combine(contentPathTextbox.Text, textureName);

                        foreach (string ext in imageExtentions)
                        {
                            string tempPath = fullPath + ext;

                            if (File.Exists(tempPath))
                            {
                                fullPath = tempPath;
                                break;
                            }
                        }

                        using (Stream stream = File.OpenRead(fullPath))
                        {
                            Image image = Image.FromFile(fullPath);
                            previewDictionary.Add(textureName, image);

                            Texture2D texture = Texture2D.FromStream(GraphicsDevice, stream);

                            textureDictionary.Add(textureName, texture);
                            textureListBox.Items.Add(textureName);
                            layer.AddTexture(texture);
                        }
                    }

                }
            }

            AdjustScrollbars(tileMap);
        }

        private void AdjustScrollbars(TileMap map)
        {
            if (map.GetWidthInPixels() > tileDisplay1.Width)
            {
                maxWidth = (int)Math.Max(map.GetWidth(), maxWidth);
                hScrollBar1.Visible = true;
                hScrollBar1.Minimum = 0;

                // TODO: Check why maxWidth is off by one, but not height
                hScrollBar1.Maximum = maxWidth + 1;
            }
            else
            {
                hScrollBar1.Visible = false;
            }

            if (map.GetHeightInPixels() > tileDisplay1.Height)
            {
                maxHeight = (int)Math.Max(map.GetHeight(), maxHeight);
                vScrollBar1.Visible = true;
                vScrollBar1.Minimum = 0;
                vScrollBar1.Maximum = maxHeight;
            }
            else
            {
                vScrollBar1.Visible = false;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (layerListBox.SelectedItem != null)
            {
                string filename = layerListBox.SelectedItem as string;
                saveFileDialog1.FileName = filename;

                TileLayer tileLayer = tileLayerDictionary[filename];

                Dictionary<int, string> utilizedTextures = new Dictionary<int, string>();
                
                foreach (string textureName in textureListBox.Items)
                {
                    int index = tileLayer.IndexOfTexture(textureDictionary[textureName]);
                    if (index != -1)
                    {
                        utilizedTextures.Add(index, textureName);
                    }
                }

                List<string> utilizedTextureList = new List<string>();

                for (int i = 0; i < utilizedTextures.Count; i++)
                    utilizedTextureList.Add(utilizedTextures[i]);

                if (saveFileDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    tileLayer.Save(saveFileDialog1.FileName, utilizedTextureList.ToArray());
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void textureListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (textureListBox.SelectedItem != null)
            {
                texturePreviewBox.Image = previewDictionary[textureListBox.SelectedItem as string];
            }
        }

        private void tileDisplay1_DeviceDraw(object sender, EventArgs e)
        {
            Logic();
            Render();
        }

        private void tileDisplay1_DeviceInitialize(object sender, EventArgs e)
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            using (Stream stream = File.OpenRead("Content/tile.png"))
            {
                tileTexture = Texture2D.FromStream(GraphicsDevice, stream);
            }

        }

        public void FillCell(int x, int y, int index)
        {
            int oldIndex = selectedLayer.GetCellIndex(x, y);

            if (oldIndex != index)
            {
                selectedLayer.SetCellIndex(x, y, index);

                // check each nieghbor
                if (x > 0 && selectedLayer.GetCellIndex(x - 1, y) == oldIndex)
                {
                    FillCell(x - 1, y, index);
                }

                if (x < selectedLayer.Width - 1 && selectedLayer.GetCellIndex(x + 1, y) == oldIndex)
                {
                    FillCell(x + 1, y, index);
                }

                if (y > 0 && selectedLayer.GetCellIndex(x, y - 1) == oldIndex)
                {
                    FillCell(x, y - 1, index);
                }

                if (y < selectedLayer.Height - 1 && selectedLayer.GetCellIndex(x, y + 1) == oldIndex)
                {
                    FillCell(x, y + 1, index);
                }
            }
        }

        private void Logic()
        {
            _camera.Position.X = hScrollBar1.Value * TileLayer.TileWidth;
            _camera.Position.Y = vScrollBar1.Value * TileLayer.TileHeight;

            int mx = Mouse.GetState().X;
            int my = Mouse.GetState().Y;
            if (selectedLayer != null)
            {
                if (mx >= 0 && mx < tileDisplay1.Width &&
                    my >= 0 && my < tileDisplay1.Height)
                {
                    cellX = mx / TileLayer.TileWidth;
                    cellY = my / TileLayer.TileHeight;

                    cellX += hScrollBar1.Value;
                    cellY += vScrollBar1.Value;

                    cellX = (int)MathHelper.Clamp(cellX, 0, selectedLayer.Width - 1);
                    cellY = (int)MathHelper.Clamp(cellY, 0, selectedLayer.Height - 1);
                }
                else
                {
                    cellX = cellY = -1;
                }

                if (Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    if (toolComboBox.SelectedItem != null)
                    {
                        // TODO : find a better way to do this
                        if ((toolComboBox.SelectedItem as string).Equals("erase", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (cellX >= 0 && cellY >= 0)
                            {
                                selectedLayer.SetCellIndex(cellX, cellY, -1);
                            }
                        }
                        else if ((toolComboBox.SelectedItem as string).Equals("draw", StringComparison.CurrentCultureIgnoreCase) && textureListBox.SelectedItem != null)
                        {
                            Texture2D texture = textureDictionary[textureListBox.SelectedItem as string];

                            int index = selectedLayer.IndexOfTexture(texture);

                            if (index == -1)
                            {
                                selectedLayer.AddTexture(texture);
                                index = selectedLayer.IndexOfTexture(texture);
                            }

                            if (cellX >= 0 && cellY >= 0)
                            {
                                selectedLayer.SetCellIndex(cellX, cellY, index);
                            }
                        }
                        else if ((toolComboBox.SelectedItem as string).Equals("fill", StringComparison.CurrentCultureIgnoreCase) && textureListBox.SelectedItem != null)
                        {
                            Texture2D texture = textureDictionary[textureListBox.SelectedItem as string];

                            int index = selectedLayer.IndexOfTexture(texture);

                            // add texture if it hasn't been added already, this should actually be an error maybe?
                            if (index == -1)
                            {
                                selectedLayer.AddTexture(texture);
                                index = selectedLayer.IndexOfTexture(texture);
                            }

                            if (cellX >= 0 && cellY >= 0)
                            {
                                FillCell(cellX, cellY, index);
                            }

                        }
                    }
                }
            }
        }

        private void Render()
        {
            GraphicsDevice.Clear(Color.Black);

            foreach (TileLayer layer in tileMap.Layers)
            {

                layer.Draw(spriteBatch, _camera);

                spriteBatch.Begin();

                for (int y = 0; y < layer.Height; y++)
                {
                    for (int x = 0; x < layer.Width; x++)
                    {
                        if (layer.GetCellIndex(x, y) == -1)
                        {
                            spriteBatch.Draw(
                                tileTexture,
                                new Rectangle(
                                    (x * TileLayer.TileWidth) - (int)_camera.Position.X,
                                    (y * TileLayer.TileHeight) - (int)_camera.Position.Y,
                                    TileLayer.TileWidth,
                                    TileLayer.TileHeight),
                                    new Color(0xff, 0xff, 0xff, 0xff));
                        }
                    }
                }
                spriteBatch.End();

                if (selectedLayer != null && layer == selectedLayer)
                {
                    break;
                }

            }

            if (selectedLayer != null)
            {
                if( cellX >= 0 && cellY >= 0 )
                {
                    spriteBatch.Begin();
                    spriteBatch.Draw(
                        tileTexture,
                        new Rectangle(
                            (cellX * TileLayer.TileWidth) - (int)_camera.Position.X,
                            (cellY * TileLayer.TileHeight) - (int)_camera.Position.Y,
                            TileLayer.TileWidth,
                            TileLayer.TileHeight),
                            Color.Red);
                    spriteBatch.End();
                }
            }
        }

        private void tileDisplay1_Resize(object sender, EventArgs e)
        {
            if (selectedLayer != null)
            {
                AdjustScrollbars(tileMap);
            }
        }

        private void layerListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (layerListBox.SelectedItem != null)
            {
                selectedLayer = tileLayerDictionary[layerListBox.SelectedItem as string];
            }
        }

        private void addLayerButton_Click(object sender, EventArgs e)
        {
            NewLayerForm form = new NewLayerForm();
            if (form.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                TileLayer tileLayer = new TileLayer(
                                int.Parse(form.width.Text),
                                int.Parse(form.height.Text));

                tileMap.Layers.Add(tileLayer);
                tileLayerDictionary.Add(form.name.Text, tileLayer);
                layerListBox.Items.Add(form.name.Text);

                AdjustScrollbars(tileMap);
            }
        }

        private void removeLayerButton_Click(object sender, EventArgs e)
        {
            if (selectedLayer != null)
            {
                string filename = layerListBox.SelectedItem as string;

                tileMap.Layers.Remove(selectedLayer);
                tileLayerDictionary.Remove(filename);
                layerListBox.Items.Remove(layerListBox.SelectedItem);

                selectedLayer = null;
            }
        }

        private void addTextureButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "JPG Images (*.jpg)|*.jpg|PNG Images (*.png)|*.png|TGA Images (*.tga)|*.tga";
            openFileDialog1.InitialDirectory = contentPathTextbox.Text;

            if (openFileDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                string filename = openFileDialog1.FileName;

                using (Stream stream = File.OpenRead(filename))
                {
                    Texture2D texture = Texture2D.FromStream(GraphicsDevice, stream);
                    Image image = Image.FromFile(filename);

                    //System.Uri uriBase = new Uri(contentPathTextbox.Text);
                    //System.Uri fullPath = new Uri(filename);

                    //Uri relativeUri = uriBase.MakeRelativeUri(fullPath);

                    // now need to strip off the ..

                    // need to get relative path from base
                    //string relativePath;
                    //filename = Path.GetFileName(r);
                    //filename = filename.Replace(contentPathTextbox.Text + Path.DirectorySeparatorChar, "");

                    string relativeFilename = filename.Replace(contentPathTextbox.Text + Path.DirectorySeparatorChar, "");

                    textureListBox.Items.Add(relativeFilename);
                    textureDictionary.Add(relativeFilename, texture);
                    previewDictionary.Add(relativeFilename, image);
                }
            }

        }

        private void removeTextureButton_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (closeImmediately)
            {
                Close();
            }
        }

        private bool dragging = false;

        private void layerListBox_MouseDown(object sender, MouseEventArgs e)
        {
            //// get selected item based on mouse point
            //int index = layerListBox.IndexFromPoint(new Point(e.X, e.Y));

            //if (index != -1)
            //{
            //    layerListBox.DoDragDrop(layerListBox.Items[index], DragDropEffects.Move);
            //    dragging = true;
            //}

        }

        private void layerListBox_DragOver(object sender, DragEventArgs e)
        {
            //e.Effect = DragDropEffects.Move;

            //Point point = layerListBox.PointToClient(new Point(e.X, e.Y));
            //int index = layerListBox.IndexFromPoint(point);

            //if (dragging && index != -1)
            //{
            //    object data = e.Data.GetData(typeof(string));
            //    layerListBox.Items.Remove(data);
            //    layerListBox.Items.Insert(index, data);
             
            //    // draw red line under
            //    layerListBox.SelectedIndex = index;
            //}
        }


        private void layerListBox_DragDrop(object sender, DragEventArgs e)
        {
            //Point point = layerListBox.PointToClient(new Point(e.X, e.Y));
            //int index = layerListBox.IndexFromPoint(point);

            //if (index != -1)
            //{
            //    object data = e.Data.GetData(typeof(string));
            //    layerListBox.Items.Remove(data);
            //    layerListBox.Items.Insert(index, data);

            //    layerListBox.SelectedIndex = index;
            //}

            dragging = false;
        }

        private void layerListBox_MouseMove(object sender, MouseEventArgs e)
        {
        }
    }
}

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DynamicLayerDemos
{
	public partial class MainPage : UserControl
	{
		ArcGISDynamicMapServiceLayer layer;
		GenerateRendererTask generateRendererTask = new GenerateRendererTask();
		public MainPage()
		{
			InitializeComponent();
			generateRendererTask.Url = "http://192.168.64.128:6080/arcgis/rest/services/roaddb/MapServer/dynamicLayer";
			generateRendererTask.ExecuteCompleted += generateRendererTask_ExecuteCompleted;
		}

		private void generateRendererTask_ExecuteCompleted(object sender, GenerateRendererResultEventArgs e)
		{
			LayerDrawingOptions layerDrawOptions = new LayerDrawingOptions();
			layerDrawOptions.LayerID = 1;
			layerDrawOptions.Renderer = e.GenerateRendererResult.Renderer;

			layer.LayerDrawingOptions =
					new LayerDrawingOptionsCollection() { layerDrawOptions };
			layer.VisibleLayers = new int[] { 1 };
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			layer = (map.Layers[0] as ArcGISDynamicMapServiceLayer);
			DynamicLayerInfoCollection myDynamicLayerInfos = layer.DynamicLayerInfos;
			if (myDynamicLayerInfos == null)
			{
				myDynamicLayerInfos = layer.CreateDynamicLayerInfosFromLayerInfos();
			}

			#region TableDataSource
			DynamicLayerInfo info = new DynamicLayerInfo()
			{
				ID = 1,
				Source = new LayerDataSource()
				{
					DataSource = new QueryDataSource()
					{
						GeometryType = ESRI.ArcGIS.Client.Tasks.GeometryType.Polyline,
						OIDFields = new string[] { "OBJECTID" },
						Query = "SELECT * FROM  SDE.china_road where FNODE_ > 1000",
						WorkspaceID = "MyDatabaseWorkspaceID"
					}
				}
			};
			#endregion

			#region QueryDataSource
			DynamicLayerInfo info = new DynamicLayerInfo()
			{
				ID = 1,
				Source = new LayerDataSource()
				{
					DataSource = new QueryDataSource()
					{
						GeometryType = ESRI.ArcGIS.Client.Tasks.GeometryType.Polyline,
						OIDFields = new string[] { "OBJECTID" },
						Query = "SELECT * FROM  SDE.china_road where FNODE_ > 1000",
						WorkspaceID = "MyDatabaseWorkspaceID"
					}
				}
			};
			#endregion

			#region UserDefine Simple Render Line
			//LayerDrawingOptions layerDrawOptions = new LayerDrawingOptions();
			//layerDrawOptions.LayerID = 1;
			//layerDrawOptions.Renderer = new SimpleRenderer()
			//{
			//	Symbol = new SimpleLineSymbol()
			//	{
			//		Color = new SolidColorBrush(Color.FromArgb((int)255, (int)0, (int)0, (int)255)),
			//		Width = 2
			//	}
			//};

			//layer.LayerDrawingOptions = new LayerDrawingOptionsCollection() { layerDrawOptions };
			#endregion

			#region UserDefine Simple Render Poly
			LayerDrawingOptions layerDrawOptions = new LayerDrawingOptions();
			layerDrawOptions.LayerID = 1;
			layerDrawOptions.Renderer = new SimpleRenderer()
			{
				Symbol = new SimpleFillSymbol()
				{
					Fill = new SolidColorBrush(Color.FromArgb((int)255, (int)0, (int)0, (int)255)),
				}
			};

			layer.LayerDrawingOptions =
					new LayerDrawingOptionsCollection() { layerDrawOptions };
			#endregion

			#region JoinDataSource
			DynamicLayerInfo info = new DynamicLayerInfo()
			{
				ID = 1,
				Source = new LayerDataSource()
				{

					DataSource = new JoinDataSource()
					{

						JoinType = JoinType.LeftInnerJoin,
						LeftTableSource = new LayerDataSource()
						{
							DataSource = new TableDataSource()
							{
								DataSourceName = "SDE.china_county",
								WorkspaceID = "MyDatabaseWorkspaceID"
							}
						},
						LeftTableKey = "OBJECTID",
						RightTableSource = new LayerDataSource()
						{
							DataSource = new TableDataSource()
							{
								DataSourceName = "SDE.china_road",
								WorkspaceID = "MyDatabaseWorkspaceID"
							}
						},
						RightTableKey = "OBJECTID"
					}
				}
			};
			#endregion

			#region RasterDataSource
			//DynamicLayerInfo info = new DynamicLayerInfo()
			//{
			//	ID = 1,
			//	Source = new LayerDataSource()
			//	{
			//		DataSource = new RasterDataSource()
			//		{
			//			DataSourceName = "rr1",
			//			WorkspaceID = "MyRasterWorkspaceID"
			//		}
			//	}
			//};
			#endregion

			myDynamicLayerInfos.Add(info);
			layer.DynamicLayerInfos = myDynamicLayerInfos;


			layer.VisibleLayers = new int[] { 1 };
			//layer.Refresh();
			//map.ZoomTo(new Envelope(11.8435360079, 49.4443060783, 11.8568721432, 49.4528247773));
			//map.ZoomTo(layer.Layers[1].e)

			#region Generate Render Class Break
			ClassBreaksDefinition classBreaksDefinition = new ClassBreaksDefinition()
			{
				ClassificationField = "FNODE_",
				ClassificationMethod = ClassificationMethod.StandardDeviation,
				BreakCount = 10,
				StandardDeviationInterval = ESRI.ArcGIS.Client.Tasks.StandardDeviationInterval.OneQuarter
			};
			classBreaksDefinition.ColorRamps.Add(new ColorRamp()
			{
				From = Colors.Blue,
				To = Colors.Red,
				Algorithm = Algorithm.HSVAlgorithm
			});

			GenerateRendererParameters rendererParams = new GenerateRendererParameters()
			{
				ClassificationDefinition = classBreaksDefinition,
				Source = info.Source
			};

			generateRendererTask.ExecuteAsync(rendererParams, rendererParams.Where);
			#endregion

			#region Generate Render Unique Value
			UniqueValueDefinition uniqueValueDefinition = new UniqueValueDefinition()
			{
				Fields = new List<string>() { "FNODE_" }
			};
			uniqueValueDefinition.ColorRamps.Add(new ColorRamp()
			{
				From = Colors.Blue,
				To = Colors.Red,
				Algorithm = Algorithm.CIELabAlgorithm
			});

			GenerateRendererParameters rendererParams = new GenerateRendererParameters()
			{
				ClassificationDefinition = uniqueValueDefinition,
				Source = info.Source
			};

			generateRendererTask.ExecuteAsync(rendererParams, rendererParams.Where);
			#endregion

		}
	}
}

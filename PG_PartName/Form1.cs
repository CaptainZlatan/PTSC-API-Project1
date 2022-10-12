using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tekla.Structures.Drawing;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Drawing.UI;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;
using TSD = Tekla.Structures.Drawing;
using T3D = Tekla.Structures.Geometry3d;
using TSDUI = Tekla.Structures.Drawing.UI;
using TSM = Tekla.Structures.Model;
using TSMUI = Tekla.Structures.Model.UI;
using System.Collections;
using Microsoft.VisualBasic;


namespace PG_PartName
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        #region Liet ket voi model
        public static TSM.Model model;
        private void Form1_Load(object sender, EventArgs e)
        {
            model = new TSM.Model();
                if (model.GetConnectionStatus())
                {
                }
                else
                {
                MessageBox.Show("Please open Tekla");
                this.Close();
                }
        }
        #endregion
        #region Change Part Name
        private void Button1_Click(object sender, EventArgs e)
        {
            TSMUI.ModelObjectSelector selector = new TSMUI.ModelObjectSelector();
            TSM.ModelObjectEnumerator objectenum = selector.GetSelectedObjects();
            foreach (TSM.Assembly assembly in objectenum)
            {
                if (assembly is TSM.Assembly)
                {
                    ArrayList weldList = new ArrayList();
                    List<int> weld_ID_List = new List<int>();
                    double assy_COG = 0;
                    double assy_TOP = 0;
                    double assy_BOT = 0;
                    double part_TOP = new double();
                    double part_BOT = new double();
                    double part_COG = new double();
                    double weldNum = 1;
                    string part_ProType = "";
                    string assy_NAME = "";
                    assembly.GetReportProperty("COG_Z", ref assy_COG);
                    assembly.GetReportProperty("ASSEMBLY_TOP_LEVEL_UNFORMATTED", ref assy_TOP);
                    assembly.GetReportProperty("ASSEMBLY_BOTTOM_LEVEL_UNFORMATTED", ref assy_BOT);
                    assembly.GetReportProperty("ASSEMBLY_NAME", ref assy_NAME);
                    //MessageBox.Show(assy_BOT.ToString());
                    ArrayList partList = Method.GetAssemblyParts(assembly);

                    double i = 0;
                    double j = 0;
                    double k = 0;
                    //MessageBox.Show(average_Part_COG.ToString());
                    foreach (TSM.Part part in partList)
                    {

                        part.GetReportProperty("TOP_LEVEL_UNFORMATTED", ref part_TOP);
                        part.GetReportProperty("BOTTOM_LEVEL_UNFORMATTED", ref part_BOT);
                        part.GetReportProperty("COG_Z", ref part_COG);
                        part.GetReportProperty("PROFILE_TYPE", ref part_ProType);
                        if ((part_ProType == "B") && (part.Position.Rotation == Position.RotationEnum.BACK || part.Position.Rotation == Position.RotationEnum.FRONT))
                        {
                            if (assy_COG < part_COG)
                            {
                                i = i + 1;
                                string part_Name = assy_NAME + "TF." + i.ToString();
                                //MessageBox.Show(part_Name);
                                part.Name = part_Name;
                                part.Modify();
                                model.CommitChanges();
                            }
                            else
                            {
                                j = j + 1;
                                string part_Name = assy_NAME + "BF." + j.ToString();
                                //MessageBox.Show(part_Name);
                                part.Name = part_Name;
                                part.Modify();
                                model.CommitChanges();
                            }
                        }
                        else
                        {
                                k = k + 1;
                                string part_Name = assy_NAME + "WB." + k.ToString();
                                //MessageBox.Show(part_Name);
                                part.Name = part_Name;
                                assembly.SetMainPart(part);
                                part.Modify();
                                assembly.Modify();
                                model.CommitChanges();
                        }

                        TSM.ModelObjectEnumerator weldList1 = part.GetWelds();
                        while (weldList1.MoveNext())
                        {
                            if ((weldList1.Current is BaseWeld) && (weldList1.Current as BaseWeld).SizeBelow != 0 && ((weldList1.Current as BaseWeld).SecondaryObject.Identifier.ID == part.Identifier.ID))
                            {
                                (weldList1.Current as BaseWeld).ReferenceText = weldNum.ToString();
                                weldNum = weldNum + 1;
                                (weldList1.Current as BaseWeld).Modify();
                                model.CommitChanges();
                            }
                        }

                    }

                }
                //int aaaaa = 0;
                //aaaaa = aaaaa + (int)((100 / objectenum.GetSize()) * 100.0f);
                //MessageBox.Show(aaaaa.ToString());
                progressBar1.Value = progressBar1.Value + (int)((100 / objectenum.GetSize()) * 1.0f);
                if (progressBar1.Value == 100)
                {
                    MessageBox.Show("DONE");
                    progressBar1.Value = 0;
                }
            }
            if (progressBar1.Value < 100 && progressBar1.Value != 0)
            {
                progressBar1.Value = (100 - progressBar1.Value) + progressBar1.Value;
                MessageBox.Show("DONE");
                progressBar1.Value = 0;
            }
        }

        private void ProgressBar1_Click(object sender, EventArgs e)
        {

        }
        #endregion
        #region Assign Material
        private void Button2_Click(object sender, EventArgs e)
        {
            Status.Visible = false;
            Processing.Visible = true;
            try
            {                
                var SelectedObjects = new Tekla.Structures.Model.UI.ModelObjectSelector();
                var List = SelectedObjects.GetSelectedObjects().ToList();
                var ListPart = List.Where(x => x is TSM.Part).ToList();
                foreach (TSM.Part item in ListPart)
                {
                    var FullCat = "";
                    var FinalCat = "";
                    item.GetReportProperty("ISC_STRUC_CAT", ref FullCat);                   
                    if (FullCat == "Primary")
                    {
                        FinalCat = "_1";
                    }
                    else if (FullCat == "Secondary")
                    {
                        FinalCat = "_2";
                    }
                    else if (FullCat == "Special")
                    {
                        FinalCat = "_S";
                    }
                    else
                    {
                        FinalCat = "";
                    }
                    var ZSteel = "";                    
                    item.GetReportProperty("ISC_Z_STEEL", ref ZSteel);
                    ZSteel = (ZSteel == "") ? "" : ("(" + ZSteel + ")");
                    var Grade = "";
                    item.GetReportProperty("ISC_STEEL_GRADE", ref Grade);
                    //Grade = (Grade == "") ? "" : "-" + Grade;
                    var FinalMaterial = item.Material.MaterialString + Grade + ZSteel + FinalCat;
                    item.Material.MaterialString = FinalMaterial;
                    item.Modify();               
                }
                var ListAssy = List.Where(y => y is Assembly).ToList();
                foreach (Assembly assy in ListAssy)
                {
                    ArrayList partList = Method.GetAssemblyParts(assy);
                    foreach (TSM.Part item in partList)
                    {
                        var FullCat = "";
                        var FinalCat = "";
                        item.GetReportProperty("ISC_STRUC_CAT", ref FullCat);
                        if (FullCat == "Primary")
                        {
                            FinalCat = "_1";
                        }
                        else if (FullCat == "Secondary")
                        {
                            FinalCat = "_2";
                        }
                        else if (FullCat == "Special")
                        {
                            FinalCat = "_S";
                        }
                        else
                        {
                            FinalCat = "";
                        }
                        var ZSteel = "";
                        item.GetReportProperty("ISC_Z_STEEL", ref ZSteel);
                        ZSteel = (ZSteel == "") ? "" : ("(" + ZSteel + ")");
                        var Grade = "";
                        item.GetReportProperty("ISC_STEEL_GRADE", ref Grade);
                        //Grade = (Grade == "") ? "" : "-" + Grade;
                        var FinalMaterial = item.Material.MaterialString + Grade + ZSteel + FinalCat;
                        item.Material.MaterialString = FinalMaterial;
                        item.Modify();
                    }
                }
            }
            catch (Exception exx)
            {

            }
            model.CommitChanges();
            Status.Visible = true;
            Processing.Visible = false;

        }
        #endregion
        #region Split Plate
        private void Button3_Click(object sender, EventArgs e)
        {
            bool esc = true;
            do
            {
                var MyPicker = new TSMUI.Picker();
                BooleanPart Cut1 = new BooleanPart();
                BooleanPart Cut2 = new BooleanPart();
                var VectorCopy = new T3D.Vector(0, 0, 0);
                try
                {
                    var Plate = MyPicker.PickObject(TSMUI.Picker.PickObjectEnum.PICK_ONE_PART, "Pick Splitted Plate (press Esc to cancel)") as TSM.Part;
                    var NewPlate = TSM.Operations.Operation.CopyObject(Plate, VectorCopy) as TSM.Part;
                    var Point1 = MyPicker.PickPoint();
                    var Point2 = MyPicker.PickPoint();
                    var Point3 = new T3D.Point(Point2.X, Point2.Y, Point2.Z + 500);

                    CutPlane CutPlane = new CutPlane();
                    CutPlane.Plane = new Plane();
                    CutPlane.Plane.Origin = Point1;
                    CutPlane.Plane.AxisX = new T3D.Vector(Point2.X - Point1.X, Point2.Y - Point1.Y, Point2.Z - Point1.Z);
                    CutPlane.Plane.AxisY = new T3D.Vector(Point3.X - Point1.X, Point3.Y - Point1.Y, Point3.Z - Point1.Z);
                    CutPlane.Father = Plate;
                    CutPlane.Insert();
                    Plate.Class = BooleanPart.BooleanOperativeClassName;

                    BooleanPart CutPlate = new BooleanPart();
                    CutPlate.Father = NewPlate;
                    CutPlate.SetOperativePart(Plate);
                    if (!CutPlate.Insert())
                    {
                        Console.WriteLine("Insert failed!");
                    }
                    NewPlate.Class = (int.Parse(NewPlate.Class) + 1).ToString();
                    NewPlate.Modify();

                }
                catch (Exception exx)
                {

                    esc = false;
                }

                model.CommitChanges();

            } while (esc == true);
        }
        #endregion
        #region Split Beam
        private void Button4_Click(object sender, EventArgs e)
        {
            bool esc = true;
            do
            {
                try
                {                    
                    var picker = new TSMUI.Picker();
                    var dsobject = picker.PickObjects(TSMUI.Picker.PickObjectsEnum.PICK_N_PARTS).ToList();
                    T3D.Point PointObject = picker.PickPoint();

                    foreach (Beam item in dsobject)
                    {
                        TSM.Operations.Operation.Split(item, PointObject);

                    }
                }

                catch (Exception exx)
                {
                    esc = false;
                }
                model.CommitChanges();
            } while (esc == true);
        }
        #endregion
        #region Creat Weld for Gap
        private void Button5_Click(object sender, EventArgs e)
        {           
            try
            {
                var picker = new TSMUI.Picker();
                var dsobject = picker.PickObjects(TSMUI.Picker.PickObjectsEnum.PICK_N_PARTS).ToList();
                var PointObject = picker.PickPoints(TSMUI.Picker.PickPointEnum.PICK_POLYGON);

                var MainPart = dsobject[0] as TSM.Part;
                var SecPart = dsobject[1] as TSM.Part;

                var PointList = new List<T3D.Point>();

                // xac dinh chieu day cua secondary
                var ThickSec = 0.0;
                var WidthSec = 0.0;
                var HeightSec = 0.0;
                var ProfileTypeSec = "";
                SecPart.GetReportProperty("PROFILE_TYPE", ref ProfileTypeSec);
                if (ProfileTypeSec=="B")
                {
                    SecPart.GetReportProperty("WIDTH", ref WidthSec);
                    SecPart.GetReportProperty("HEIGHT", ref HeightSec);
                    ThickSec = Math.Min(WidthSec, HeightSec);
                }
                else
                {
                    SecPart.GetReportProperty("PROFILE.PLATE_THICKNESS", ref ThickSec);
                }
                // Tao moi han
                var WeldType = "Bevel";
                if (WeldType== "Bevel")
                {
                    if (ThickSec>=25)
                    {
                        var Weld = Method.Weld(MainPart, SecPart, PointObject, "4", "4", ThickSec, 0, true);
                    }
                    else
                    {
                        var Weld = Method.Weld(MainPart, SecPart, PointObject, "4", "0", ThickSec, 0, true);
                    }
                    
                }
                else
                {
                    var Weld = Method.Weld(MainPart, SecPart, PointObject, "10", "0", ThickSec, 0, true);
                }

                //Toa gap
                foreach (var item in PointObject)
                {                    
                    PointList.Add(item as T3D.Point);
                }
                if (PointList.Count<=2)
                {
                    var CutBeam = Method.Beam(PointList[0], PointList[1]);
                    Method.PartCut(SecPart, CutBeam);
                }
                else
                {
                    var CutBeam = Method.PolyBeam(PointList);
                    Method.PartCut(SecPart, CutBeam);
                }                             
               
            }
            catch (Exception exx)
            {

            }
            model.CommitChanges();            

        }
        #endregion
        #region Change Dwg. NAME
        private void Button6_Click(object sender, EventArgs e)
        {
            TSD.DrawingHandler drawingHandler = new DrawingHandler();
            TSD.DrawingEnumerator drawingEnum = drawingHandler.GetDrawingSelector().GetSelected();
            //string drawingNameInput = Interaction.InputBox("Input Drawing Name", "Drawing Name", "HL2_HL-OSS-SMPTSC-STR-DWG", -1, -1);
            foreach (TSD.Drawing drawing in drawingEnum)
            {
                string assyName = "";
                string assyNameNum = "";
                string assyDrawingName = "";
                ArrayList partList = new ArrayList();
                string assyPos = "";
                string drawingName = "";
                string projectNum = "";

                if (drawing is AssemblyDrawing)
                {
                    TSD.DrawingObjectEnumerator drawingObjectEnum = drawing.GetSheet().GetAllObjects(typeof(TSD.Part));
                    while (drawingObjectEnum.MoveNext())
                    {

                            TSD.Part partDrawing = drawingObjectEnum.Current as TSD.Part;
                            TSM.Part partModel = model.SelectModelObject(partDrawing.ModelIdentifier) as TSM.Part;
                            partModel.GetReportProperty("ASSEMBLY.ASSEMBLY_POS", ref assyPos);
                            if (drawing.Mark != assyPos || partModel.Material.MaterialString == "DUMMY" )
                            {
                            partList.Add(drawingObjectEnum.Current);
                            }
                    }
                    foreach (TSD.Part part in partList)
                    {
                        TSM.Part partModel = model.SelectModelObject(part.ModelIdentifier) as TSM.Part;
                        partModel.GetReportProperty("ASSEMBLY.ASSEMBLY_NAME", ref assyName);
                        string projectName = "";
                        partModel.GetReportProperty("PROJECT.OBJECT", ref projectName);
                        projectNum = (projectName.Contains("HL2")) ? "5" : "6";
                        assyNameNum = assyName.Substring(assyName.LastIndexOf(".") + 1, (assyName.Length - assyName.LastIndexOf(".")) - 1);
                        assyDrawingName = assyName.Substring(0, 4);
                        if (assyNameNum.Length == 1)
                        {
                            drawingName = projectName.Substring(0, 3) + "_HL-OSS-SMPTSC-STR-DWG-" + projectNum + assyDrawingName + "-AS-00" + assyNameNum;
                        }
                        else if (assyNameNum.Length == 2)
                        {
                            drawingName = projectName.Substring(0, 3) + "_HL-OSS-SMPTSC-STR-DWG-" + projectNum + assyDrawingName + "-AS-0" + assyNameNum;
                        }
                        else
                        {
                            drawingName = projectName.Substring(0, 3) + "_HL-OSS-SMPTSC-STR-DWG-" + projectNum + assyDrawingName + "-AS-" + assyNameNum;
                        }

                        break;
                    }
                }
                drawing.Name = drawingName;
                drawing.Modify();
                drawing.CommitChanges();
                progressBar1.Value = progressBar1.Value + (int)((100 / drawingEnum.GetSize()) * 1.0f);
                if (progressBar1.Value == 100)
                {
                    MessageBox.Show("DONE");
                    progressBar1.Value = 0;
                }
            }
            if (progressBar1.Value < 100 && progressBar1.Value != 0)
            {
                progressBar1.Value = (100 - progressBar1.Value) + progressBar1.Value;
                MessageBox.Show("DONE");
                progressBar1.Value = 0;
            }

        }
        #endregion
    }
}

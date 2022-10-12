using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Model;
using System.Collections;
using TSM = Tekla.Structures.Model;
using T3D = Tekla.Structures.Geometry3d;
using TSMUI = Tekla.Structures.Model.UI;
using TSD = Tekla.Structures.Drawing;

namespace PG_PartName
{
    public static class Method
    {
        public static List<ModelObject> ToList(this ModelObjectEnumerator enumerator)
        {
            // chuyen sang list
            var List = new List<ModelObject>();
            while (enumerator.MoveNext())
            {
                List.Add(enumerator.Current);
            }
            return List;
        }
        public static ArrayList GetAssemblyParts(Assembly assembly)
        {
            ArrayList list = new ArrayList();
            IEnumerator enumerator = assembly.GetSecondaries().GetEnumerator();
            list.Add(assembly.GetMainPart());
            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Current as ModelObject);
            }
            return list;
        }
        public static ArrayList GetDrawingParts(TSD.Drawing drawing)
        {
            ArrayList list = new ArrayList();
            IEnumerator enumerator = drawing.GetSheet().GetAllObjects(typeof(Part));
            string drawingMark = drawing.Mark;
            while (enumerator.MoveNext())
            {

                list.Add(enumerator.Current as ModelObject);
            }
            return list;
        }
        public static ArrayList GetPartWelds(Part part)
        {
            ArrayList list = new ArrayList();
            IEnumerator enumerator = part.GetWelds().GetEnumerator();
            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Current as Weld);
            }
            return list;
        }
        public static ArrayList GetUniqueWelds(ArrayList weldList)
        {
            ArrayList weldList1 = new ArrayList();
            IEnumerator<TSM.Weld> enumeratorWeld = weldList.Cast<TSM.Weld>().Distinct().GetEnumerator();
            while (enumeratorWeld.MoveNext())
            {
                weldList1.Add(enumeratorWeld.Current as Weld);
            }
            return weldList1;
        }
        public static PolygonWeld Weld(TSM.Part MainPart, TSM.Part SecondPart, ArrayList PointList, string WeldTypeAbove, string WeldTypeBelow, double SizeAbove, double SizeBelow, bool SiteShop)
        {
            PolygonWeld PolygonWeld = new PolygonWeld();
            PolygonWeld.MainObject = MainPart;
            PolygonWeld.SecondaryObject = SecondPart;
            for (int i = 0; i < PointList.Count; i++)
            {
                T3D.Point WeldP = PointList[i] as T3D.Point;
                PolygonWeld.Polygon.Points.Add(WeldP);
            }
            PolygonWeld.TypeAbove = Method.WeldType_Translating(WeldTypeAbove);
            PolygonWeld.SizeAbove = SizeAbove;
            PolygonWeld.TypeBelow = Method.WeldType_Translating(WeldTypeBelow);
            PolygonWeld.SizeBelow = SizeBelow;
            PolygonWeld.AroundWeld = false;
            PolygonWeld.ShopWeld = SiteShop;
            PolygonWeld.Insert();
            if (SiteShop==false)
            {
                PolygonWeld.SetUserProperty("WELD_PM2", MainPart.Name);
                PolygonWeld.SetUserProperty("WELD_MATERIAL_PM2", MainPart.Material.MaterialString);
                PolygonWeld.Modify();
            }            
            return PolygonWeld;
        }
        public static BaseWeld.WeldTypeEnum WeldType_Translating(string _type)
        {
            return ((_type != "0") ? ((_type != "1") ? ((_type != "2") ? ((_type != "3") ? ((_type != "4") ? ((_type != "5") ? ((_type != "6") ? ((_type != "7") ? ((_type != "8") ? ((_type != "9") ? ((_type != "10") ? ((_type != "11") ? ((_type != "12") ? ((_type != "13") ? ((_type != "14") ? ((_type != "15") ? ((_type != "16") ? ((_type != "17") ? ((_type != "18") ? ((_type != "19") ? ((_type != "20") ? ((_type != "21") ? ((_type != "22") ? ((_type != "23") ? ((_type != "24") ? ((_type != "25") ? ((_type != "26") ? BaseWeld.WeldTypeEnum.WELD_TYPE_NONE : BaseWeld.WeldTypeEnum.WELD_TYPE_INCLINED) : BaseWeld.WeldTypeEnum.WELD_TYPE_FOLD) : BaseWeld.WeldTypeEnum.WELD_TYPE_ISO_SURFACING) : BaseWeld.WeldTypeEnum.WELD_TYPE_EDGE) : BaseWeld.WeldTypeEnum.STEEP_FLANKED_BEVEL_GROOVE_SINGLE_BEVEL_BUTT) : BaseWeld.WeldTypeEnum.STEEP_FLANKED_BEVEL_GROOVE_SINGLE_V_BUTT) : BaseWeld.WeldTypeEnum.WELD_TYPE_MELT_THROUGH) : BaseWeld.WeldTypeEnum.WELD_TYPE_PARTIAL_PENETRATION_SQUARE_GROOVE_PLUS_FILLET) : BaseWeld.WeldTypeEnum.WELD_TYPE_PARTIAL_PENETRATION_SINGLE_BEVEL_BUTT_PLUS_FILLET) : BaseWeld.WeldTypeEnum.WELD_TYPE_CORNER_FLANGE) : BaseWeld.WeldTypeEnum.WELD_TYPE_FLARE_V_GROOVE) : BaseWeld.WeldTypeEnum.WELD_TYPE_FLARE_BEVEL_GROOVE) : BaseWeld.WeldTypeEnum.WELD_TYPE_SLOT) : BaseWeld.WeldTypeEnum.WELD_TYPE_SEAM) : BaseWeld.WeldTypeEnum.WELD_TYPE_SPOT) : BaseWeld.WeldTypeEnum.WELD_TYPE_PLUG) : BaseWeld.WeldTypeEnum.WELD_TYPE_FILLET) : BaseWeld.WeldTypeEnum.WELD_TYPE_BEVEL_BACKING) : BaseWeld.WeldTypeEnum.WELD_TYPE_J_GROOVE_J_BUTT) : BaseWeld.WeldTypeEnum.WELD_TYPE_U_GROOVE_SINGLE_U_BUTT) : BaseWeld.WeldTypeEnum.WELD_TYPE_SINGLE_BEVEL_BUTT_WITH_BROAD_ROOT_FACE) : BaseWeld.WeldTypeEnum.WELD_TYPE_SINGLE_V_BUTT_WITH_BROAD_ROOT_FACE) : BaseWeld.WeldTypeEnum.WELD_TYPE_BEVEL_GROOVE_SINGLE_BEVEL_BUTT) : BaseWeld.WeldTypeEnum.WELD_TYPE_BEVEL_GROOVE_SINGLE_V_BUTT) : BaseWeld.WeldTypeEnum.WELD_TYPE_SQUARE_GROOVE_SQUARE_BUTT) : BaseWeld.WeldTypeEnum.WELD_TYPE_EDGE_FLANGE) : BaseWeld.WeldTypeEnum.WELD_TYPE_NONE);
        }
        public static PolyBeam PolyBeam(List<T3D.Point> PointList)
        {
            PolyBeam PolyBeam = new PolyBeam();

            foreach (var item in PointList)
            {
                ContourPoint CP = new ContourPoint(item, null);
                PolyBeam.AddContourPoint(CP);
            }

            PolyBeam.Profile.ProfileString = "PL6X100";
            PolyBeam.Material.MaterialString = "S355JR";
            PolyBeam.Position.Plane = Position.PlaneEnum.MIDDLE;
            PolyBeam.Position.Rotation = Position.RotationEnum.FRONT;
            PolyBeam.Position.Depth = Position.DepthEnum.MIDDLE;
            bool Result = false;
            Result = PolyBeam.Insert();
            return PolyBeam;
        }
        public static Beam Beam(T3D.Point Sp, T3D.Point Ep)
        {
            Beam Beam = new Beam();

            Beam.StartPoint = Sp;
            Beam.EndPoint = Ep;
            Beam.Profile.ProfileString = "PL6X100";
            Beam.Material.MaterialString = "S355JR";
            Beam.Position.Plane = Position.PlaneEnum.MIDDLE;
            Beam.Position.Rotation = Position.RotationEnum.TOP;
            Beam.Position.Depth = Position.DepthEnum.MIDDLE;
            bool Result = false;
            Result = Beam.Insert();
            return Beam;
        }
        public static void PartCut(Part MainPart, Part CutPart)
        {
            BooleanPart Beam = new BooleanPart();
            CutPart.Class = BooleanPart.BooleanOperativeClassName;
            Beam.Father = MainPart;
            Beam.SetOperativePart(CutPart);
            if (!Beam.Insert())
                Console.WriteLine("Insert failed!");
            CutPart.Delete();
        }


    }
}

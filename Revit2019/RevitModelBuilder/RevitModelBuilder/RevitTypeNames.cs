using System;
using System.Collections.Generic;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace RevitModelBuilder {

    // Global Enums
    public enum RevitOutputType {
        Area, CurveByPoints, DetailLine, DetailArc, DetailNurbSpline, FamilyInstance, FilledRegion, Floor, Grid, 
        ModelLine, ModelArc, ModelNurbSpline, ReferencePoint, Form, Room, Wall,
    }
    public enum RevitCurveType {Line, Arc, Ellipse, HermiteSpline, NurbSpline}; 

    // Class Dictionaries and Lists
    public class RevitTypeNames {

        public Dictionary<Type, RevitOutputType> RevitOutputTypeDictionary = new Dictionary<Type, RevitOutputType> {
            {typeof(DetailLine), RevitOutputType.DetailLine},
            {typeof(DetailArc), RevitOutputType.DetailArc},
            {typeof(DetailNurbSpline), RevitOutputType.DetailNurbSpline},
            {typeof(ModelLine), RevitOutputType.ModelLine},
            {typeof(ModelArc), RevitOutputType.ModelArc},
            {typeof(ModelNurbSpline), RevitOutputType.ModelNurbSpline},
            {typeof(Wall), RevitOutputType.Wall},
            {typeof(FamilyInstance), RevitOutputType.FamilyInstance},
            {typeof(FilledRegion), RevitOutputType.FilledRegion},
            {typeof(Floor), RevitOutputType.Floor},           
            {typeof(ReferencePoint), RevitOutputType.ReferencePoint},
            {typeof(CurveByPoints), RevitOutputType.CurveByPoints},
            {typeof(Form), RevitOutputType.Form},
            {typeof(Room), RevitOutputType.Room},
            {typeof(Area), RevitOutputType.Area}
        };

        public Dictionary<Type, RevitCurveType> RevitCurveTypeDictionary = new Dictionary<Type, RevitCurveType> {
            {typeof(Line), RevitCurveType.Line},
            {typeof(Arc), RevitCurveType.Arc},
            {typeof(Ellipse), RevitCurveType.Ellipse},
            {typeof(HermiteSpline), RevitCurveType.HermiteSpline}, 
            {typeof(NurbSpline), RevitCurveType.NurbSpline}             
        };

        public List<object> SimpleRevitOutputTypes = new List<object> {
            typeof(DetailLine),
            typeof(DetailArc),
            typeof(DetailNurbSpline),
            typeof(ModelLine),
            typeof(ModelArc),
            typeof(ModelNurbSpline),
            typeof(Wall),
            typeof(FamilyInstance),
            typeof(FilledRegion),
            typeof(Floor),            
            typeof(ReferencePoint),
            typeof(CurveByPoints),
            typeof(Form),
            typeof(Room),
            typeof(Area)
        };
        public List<object> SketchRevitOutputTypes = new List<object> {
            typeof(Wall),
            typeof(Floor),
            typeof(Room),
            typeof(Area),
            typeof(FilledRegion)
        };
        public List<object> NurbSplineRevitOutputTypes = new List<object> {
            typeof(DetailNurbSpline),
            typeof(ModelNurbSpline)
        };

    }
}

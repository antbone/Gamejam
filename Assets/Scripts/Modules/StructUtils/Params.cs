
// public class VParams
// {
//     public ERelativeType? relativeType = null;
//     public ELayer? layer = null;
//     public object data;
//     public static Group Relative(ERelativeType type) => new Group() { paramInfo = new VParams() { relativeType = type } };
//     public static Group Layer(ELayer layer) => new Group() { paramInfo = new VParams() { layer = layer } };
//     public static Group Data(object data) => new Group() { paramInfo = new VParams() { data = data } };
//     public class Group
//     {
//         public VParams paramInfo;
//         public Group Relative(ERelativeType type)
//         {
//             paramInfo.relativeType = type;
//             return this;
//         }
//         public Group Layer(ELayer layer)
//         {
//             paramInfo.layer = layer;
//             return this;
//         }
//         public Group Data(object data)
//         {
//             paramInfo.data = data;
//             return this;
//         }
//     }
// }
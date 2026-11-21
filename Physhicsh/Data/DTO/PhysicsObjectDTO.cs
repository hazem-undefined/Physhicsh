using Physhicsh.Json.DTO;

namespace Physhicsh.Data.DTO;

public class PhysicsObjectDTO
{
    public PhysicsObjectDTO(BodySaveDTO bodyDTO, ShapeSaveDTO shapeDTO)
    {
        BodyDTO = bodyDTO;
        ShapeDTO = shapeDTO;
    }

    public BodySaveDTO BodyDTO { get; set; }
    public ShapeSaveDTO ShapeDTO { get; set; }
}

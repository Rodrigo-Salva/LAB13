using AutoMapper;
using Veterinaria.Entidades.Dtos;
using Veterinaria.Entidades.Models;

namespace Veterinaria.Negocio.Mapeos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Entidad -> DTO de lectura (lo que la API devuelve)
        CreateMap<Dueno, DuenoDto>()
            .ForMember(d => d.CantidadMascotas, o => o.MapFrom(s => s.Mascotas.Count));

        CreateMap<Mascota, MascotaDto>()
            .ForMember(d => d.DuenoNombre, o => o.MapFrom(s => s.Dueno.Nombre));

        // DTOs de entrada -> Entidad (lo que llega del cliente)
        CreateMap<DuenoCrearDto, Dueno>();
        CreateMap<DuenoActualizarDto, Dueno>();   // se usa con Map(origen, destino) sobre la entidad existente
        CreateMap<MascotaCrearDto, Mascota>();
        CreateMap<MascotaActualizarDto, Mascota>();
    }
}

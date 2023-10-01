using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPH2DParamsGenerator
{
    internal class ExperimentParams
    {
        public int params_generator_version_major { get; set; }
        public int params_generator_version_minor { get; set; }

        public int? density_treatment { get; set; }
        public int? density_normalization { get; set; }
        public int? density_skf { get; set; }

        public int eos_sound_vel_method { get; set; }
        public float? eos_sound_vel { get; set; }
        public float? eos_sound_vel_coef { get; set; }

        public float? intf_hsml_coef { get; set; }
        public int? intf_sph_approximation { get; set; }
        public int? intf_skf { get; set; }

        public bool? visc { get; set; }
        public float? visc_coef { get; set; }

        public bool? artificial_viscosity { get; set; }
        public float? artificial_shear_visc { get; set; }
        public float? artificial_bulk_visc { get; set; }
        public int? artificial_viscosity_skf { get; set; }

        public bool? average_velocity { get; set; }
        public float? average_velocity_coef { get; set; }
        public int? average_velocity_skf { get; set; }

        public int? step_treatment { get; set; }
        public int? save_step { get; set; }
        public int? dump_step { get; set; }
        public float? save_time { get; set; }
        public float? dump_time { get; set; }
        public int? step_time_estimate { get; set; }
        public bool? use_dump { get; set; }
        public bool? use_custom_time_estimate_step { get; set; }

        public bool? consistency_check { get; set; }
        public int? consistency_check_step { get; set; }
        public int? consistency_treatment { get; set; }

        public int boundary_treatment { get; set; }

        public int? nwm { get; set; }
        public float? nwm_wait { get; set; }
        public float? nwm_wave_length { get; set; }
        public float? nwm_wave_magnitude { get; set; }
        public float? nwm_piston_magnitude { get; set; }

        public float simulation_time { get; set; }
        public int? dt_correction_method { get; set; }
        public float? dt { get; set; }
        public float? CFL_coef { get; set; }

        public int? max_neighbours { get; set; }
        public int? local_threads { get; set; }
    }
}

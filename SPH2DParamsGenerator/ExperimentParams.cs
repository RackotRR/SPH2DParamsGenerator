namespace SPH2DParamsGenerator
{
    class ExperimentParams
    {
        public uint version_major { get; set; }
        public uint version_minor { get; set; }
        public uint SPH2D_version_major { get; set; }
        public uint SPH2D_version_minor { get; set; }
        public uint SPH2D_version_patch { get; set; }
        public uint dim { get; set; }
        public uint maxn { get; set; }
        public uint max_neighbours { get; set; }
        public uint max_cells { get; set; }
        public float x_maxgeom { get; set; }
        public float x_mingeom { get; set; }
        public float y_maxgeom { get; set; }
        public float y_mingeom { get; set; }
        public uint x_fluid_particles { get; set; }
        public uint y_fluid_particles { get; set; }
        public float x_fluid_min { get; set; }
        public float y_fluid_min { get; set; }
        public float x_fluid_max { get; set; }
        public float y_fluid_max { get; set; }
        public float x_boundary_min { get; set; }
        public float y_boundary_min { get; set; }
        public float x_boundary_max { get; set; }
        public float y_boundary_max { get; set; }
        public uint nfluid { get; set; }
        public uint nvirt { get; set; }
        public uint ntotal { get; set; }
        public float wave_length { get; set; }
        public float depth { get; set; }
        public float freq { get; set; }
        public float piston_amp { get; set; }
        public float wave_amp { get; set; }
        public float wave_number { get; set; }
        public float beach_x { get; set; }
        public uint nwm_particles_start { get; set; }
        public uint nwm_particles_end { get; set; }
        public float generator_time_wait { get; set; }
        public float CFL_coef { get; set; }
        public float dt { get; set; }
        public uint dt_correction_method { get; set; }
        public float simulation_time { get; set; }
        public uint local_threads { get; set; }
        public float eos_csqr_k { get; set; }
        public uint eos_sound_vel_method { get; set; }
        public float eos_sound_vel { get; set; }
        public uint pa_sph { get; set; }
        public uint density_skf { get; set; }
        public uint int_force_skf { get; set; }
        public uint average_velocity_skf { get; set; }
        public uint artificial_viscosity_skf { get; set; }
        public float cell_scale_k { get; set; }
        public uint nwm { get; set; }
        public bool waves_generator { get; set; }
        public uint boundary_layers_num { get; set; }
        public uint sbt { get; set; }
        public bool use_chess_order { get; set; }
        public float hsml { get; set; }
        public float delta { get; set; }
        public float boundary_delta { get; set; }
        public bool summation_density { get; set; }
        public bool nor_density { get; set; }
        public bool average_velocity { get; set; }
        public float average_velocity_epsilon { get; set; }
        public bool visc { get; set; }
        public float water_dynamic_visc { get; set; }
        public bool artificial_viscosity { get; set; }
        public float artificial_shear_visc { get; set; }
        public float artificial_bulk_visc { get; set; }
        public int TYPE_BOUNDARY { get; set; }
        public int TYPE_NON_EXISTENT { get; set; }
        public int TYPE_WATER { get; set; }
        public float mass { get; set; }
        public float rho0 { get; set; }
        public bool enable_check_consistency { get; set; }
        public bool inf_stop { get; set; }
        public uint starttimestep { get; set; }
        public uint maxtimestep { get; set; }
        public uint normal_check_step { get; set; }
        public uint save_step { get; set; }
        public uint dump_step { get; set; }
        public uint print_time_est_step { get; set; }
        public uint stepping_treatment { get; set; }
        public float pi { get; set; }
        public float g { get; set; }
        public string experiment_name { get; set; }
        public string format_line { get; set; }
    }
}


using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace SPH2DParamsGenerator
{
    public partial class Form1 : Form
    {
        string LastFolderOpened = null;
        const uint TargetParamsVersionMajor = 2;
        const uint TargetParamsVersionMinor = 9;

        public Form1()
        {
            InitializeComponent();

            Setup();
        }

        void Setup()
        {
            comboBox_ParticlesGenerator.Items.Clear();
            comboBox_ParticlesGenerator.Items.Add("None");
            comboBox_ParticlesGenerator.Items.Add("SPH2DPicGen");
            comboBox_ParticlesGenerator.SelectedIndex = 1;
            comboBox_ParticlesGenerator.DropDownStyle = ComboBoxStyle.DropDownList;

            SetupAverageVelocity();
            SetupArtificialViscosity();
            SetupDynamicViscosity();
            SetupWavesGenerator();
            SetupBoundaries();
            SetupTimeIntegration();
            SetupDensity();
            SetupInternalForce();
        }

        void SetupSKFItems(ComboBox.ObjectCollection comboBoxItems)
        {
            comboBoxItems.Clear();
            comboBoxItems.Add("Cubic");
            comboBoxItems.Add("Gauss");
            comboBoxItems.Add("Quintic");
            comboBoxItems.Add("Desbrun");
        }

        void SetupArtificialViscosity()
        {
            SetupSKFItems(comboBox_ArtViscSKF.Items);
            comboBox_ArtViscSKF.SelectedIndex = 0;
            comboBox_ArtViscSKF.DropDownStyle = ComboBoxStyle.DropDownList;

            UpdateArtificialViscosity();
        }
        void UpdateArtificialViscosity()
        {
            bool enabled = checkBox_EnableArtVisc.Checked;
            comboBox_ArtViscSKF.Enabled = enabled;
            textBox_ArtViscBulk.Enabled = enabled;
            textBox_ArtViscShear.Enabled = enabled;
        }

        void SetupAverageVelocity()
        {
            SetupSKFItems(comboBox_AvVelSKF.Items);
            comboBox_AvVelSKF.SelectedIndex = 0;
            comboBox_AvVelSKF.DropDownStyle = ComboBoxStyle.DropDownList;

            UpdateAverageVelocity();
        }
        void UpdateAverageVelocity()
        {
            bool enabled = checkBox_EnableAvVel.Checked;
            comboBox_AvVelSKF.Enabled = enabled;
            textBox_AvVelCoef.Enabled = enabled;
        }

        void SetupDynamicViscosity()
        {
            UpdateDynamicViscosity();
        }
        void UpdateDynamicViscosity()
        {
            textBox_DynViscValue.Enabled = checkBox_EnableDynVisc.Checked;
        }

        void SetupWavesGenerator()
        {
            comboBox_WavesGenTreat.Items.Clear();
            comboBox_WavesGenTreat.Items.Add("RZM");
            comboBox_WavesGenTreat.Items.Add("Dynamic");
            comboBox_WavesGenTreat.Items.Add("Impulse");
            comboBox_WavesGenTreat.SelectedIndex = 2;
            comboBox_WavesGenTreat.DropDownStyle = ComboBoxStyle.DropDownList;
            UpdateWavesGenerator();
        }
        void UpdateWavesGenerator()
        {
            bool enabled = checkBox_EnableWavesGen.Checked;
            comboBox_WavesGenTreat.Enabled = enabled;
            textBox_WavesGenFreq.Enabled = enabled;
            textBox_WavesGenLen.Enabled = enabled;
            textBox_WavesGenMagnitude.Enabled = enabled;
            textBox_WavesGenTimeWait.Enabled = enabled;
        }

        void SetupBoundaries()
        {
            comboBox_BoundaryTreat.Items.Clear();
            comboBox_BoundaryTreat.Items.Add("Dynamic");
            comboBox_BoundaryTreat.Items.Add("Repulsive");
            comboBox_BoundaryTreat.SelectedIndex = 1;
            comboBox_BoundaryTreat.DropDownStyle = ComboBoxStyle.DropDownList;
            UpdateBoundaries();
        }
        void UpdateBoundaries()
        {
            textBox_BoundaryLayersNum.Enabled = comboBox_ParticlesGenerator.SelectedIndex == 0;
        }

        void SetupTimeIntegration()
        {
            comboBox_TimeTreat.Items.Clear();
            comboBox_TimeTreat.Items.Add("Const: value");
            comboBox_TimeTreat.Items.Add("Const: estimate");
            comboBox_TimeTreat.Items.Add("Corrective");
            comboBox_TimeTreat.SelectedIndex = 1;
            comboBox_TimeTreat.DropDownStyle = ComboBoxStyle.DropDownList;
            UpdateTimeIntegration();
        }
        void UpdateTimeIntegration()
        {
            int index = comboBox_TimeTreat.SelectedIndex;
            textBox_TimeDT.Enabled = index == 0; // (Const: value)
            textBox_TimeCFL.Enabled = index != 0; // not (Const: value)
        }

        void SetupDensity()
        {
            comboBox_DensityTreat.Items.Clear();
            comboBox_DensityTreat.Items.Add("Summation");
            comboBox_DensityTreat.Items.Add("Continuity");
            comboBox_DensityTreat.SelectedIndex = 0;
            comboBox_DensityTreat.DropDownStyle = ComboBoxStyle.DropDownList;

            SetupSKFItems(comboBox_DensitySKF.Items);
            comboBox_DensitySKF.SelectedIndex = 0;
            comboBox_DensitySKF.DropDownStyle = ComboBoxStyle.DropDownList;

            UpdateDensity();
        }
        void UpdateDensity()
        {
            bool summationTreatment = comboBox_DensityTreat.SelectedIndex == 0;
            checkBox_DensityNorm.Enabled = summationTreatment;
        }

        void SetupInternalForce()
        {
            comboBox_IntForceTreat.Items.Clear();
            comboBox_IntForceTreat.Items.Add("(pi + pj) / (\u03c1i * \u03c1j)");
            comboBox_IntForceTreat.Items.Add("pi/\u03c1j2 + pj/\u03c1j2");
            comboBox_IntForceTreat.SelectedIndex = 1;
            comboBox_IntForceTreat.DropDownStyle = ComboBoxStyle.DropDownList;

            SetupSKFItems(comboBox_IntForceSKF.Items);
            comboBox_IntForceSKF.SelectedIndex = 0;
            comboBox_IntForceSKF.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        void FillInArtificialViscosity(ExperimentParams experimentParams)
        {
            experimentParams.artificial_viscosity = checkBox_EnableArtVisc.Checked;
            if (experimentParams.artificial_viscosity)
            {
                experimentParams.artificial_bulk_visc = float.Parse(textBox_ArtViscBulk.Text);
                experimentParams.artificial_shear_visc = float.Parse(textBox_ArtViscShear.Text);
                experimentParams.artificial_viscosity_skf = (uint)comboBox_ArtViscSKF.SelectedIndex;
            }
            else
            {
                experimentParams.artificial_bulk_visc = 0;
                experimentParams.artificial_shear_visc = 0;
                experimentParams.artificial_viscosity_skf = 0;
            }
        }
        void FillInAverageVelocity(ExperimentParams experimentParams)
        {
            experimentParams.average_velocity = checkBox_EnableAvVel.Checked;
            if (experimentParams.average_velocity)
            {
                experimentParams.average_velocity_epsilon = float.Parse(textBox_AvVelCoef.Text);
                experimentParams.average_velocity_skf = (uint)comboBox_AvVelSKF.SelectedIndex;
            }
            else
            {
                experimentParams.average_velocity_epsilon = 0;
                experimentParams.average_velocity_skf = 0;
            }
        }
        void FillInDensity(ExperimentParams experimentParams)
        {
            experimentParams.summation_density = comboBox_DensityTreat.SelectedIndex == 0;
            experimentParams.density_skf = (uint)comboBox_DensitySKF.SelectedIndex;
            experimentParams.mass = (float)Math.Pow(float.Parse(textBox_GeomDelta.Text), 2) * float.Parse(textBox_DensityValue.Text);
            experimentParams.nor_density = checkBox_DensityNorm.Checked;
        }
        void FillInInternalForce(ExperimentParams experimentParams)
        {
            experimentParams.eos_csqr_k = float.Parse(textBox_IntForceSoundVelCoef.Text);
            experimentParams.int_force_skf = (uint)comboBox_IntForceSKF.SelectedIndex;
            experimentParams.pa_sph = (uint)comboBox_IntForceTreat.SelectedIndex;
        }
        void FillInGeometry(ExperimentParams experimentParams)
        {
            experimentParams.delta = float.Parse(textBox_GeomDelta.Text);
            experimentParams.hsml = experimentParams.delta * float.Parse(textBox_GeomSmoothLen.Text);
            experimentParams.x_mingeom = float.Parse(textBox_GeomOriginX.Text);
            experimentParams.y_mingeom = float.Parse(textBox_GeomOriginY.Text);
        }
        void FillInWavesGenerator(ExperimentParams experimentParams)
        {
            experimentParams.waves_generator = checkBox_EnableWavesGen.Checked;
            if (experimentParams.waves_generator)
            {
                experimentParams.nwm = (uint)comboBox_WavesGenTreat.SelectedIndex;
                experimentParams.freq = float.Parse(textBox_WavesGenFreq.Text);
                experimentParams.generator_time_wait = float.Parse(textBox_WavesGenTimeWait.Text);
                experimentParams.wave_amp = float.Parse(textBox_WavesGenMagnitude.Text);
                experimentParams.wave_length = float.Parse(textBox_WavesGenLen.Text);
                experimentParams.wave_number = 2.0f * experimentParams.pi / experimentParams.wave_length;
            }
            else
            {
                experimentParams.nwm = 0;
                experimentParams.freq = 0;
                experimentParams.generator_time_wait = 0;
                experimentParams.wave_amp = 0;
                experimentParams.wave_length = 0;
                experimentParams.wave_number = 0;
            }
        }
        void FillInBoundaries(ExperimentParams experimentParams)
        {
            if (comboBox_ParticlesGenerator.SelectedIndex == 0)
            {
                experimentParams.boundary_layers_num = uint.Parse(textBox_BoundaryLayersNum.Text);
            }
            else
            {
                experimentParams.boundary_layers_num = 0;
            }
            experimentParams.boundary_delta = experimentParams.delta * float.Parse(textBox_BoundaryDelta.Text);
            experimentParams.sbt = (uint)comboBox_BoundaryTreat.SelectedIndex;
        }
        void FillInTimeIntegration(ExperimentParams experimentParams)
        {
            experimentParams.dt_correction_method = (uint)comboBox_TimeTreat.SelectedIndex;
            experimentParams.simulation_time = float.Parse(textBox_TimeSim.Text);
            if (comboBox_TimeTreat.SelectedIndex == 0)
            {
                experimentParams.dt = float.Parse(textBox_TimeDT.Text);
            }
            else
            {
                experimentParams.dt = 0;
            }
        }
        void FillInTimeStep(ExperimentParams experimentParams)
        {
            experimentParams.dump_step = uint.Parse(textBox_StepDump.Text);
            experimentParams.normal_check_step = uint.Parse(textBox_StepCheck.Text);
            experimentParams.print_time_est_step = uint.Parse(textBox_StepEstimate.Text);
            experimentParams.save_step = uint.Parse(textBox_StepSave.Text);
        }
        void FillInViscosity(ExperimentParams experimentParams)
        {
            experimentParams.visc = checkBox_EnableDynVisc.Checked;
            if (experimentParams.visc)
            {
                experimentParams.water_dynamic_visc = float.Parse(textBox_DynViscValue.Text);
            }
            else
            {
                experimentParams.water_dynamic_visc = 0;
            }
        }
        void FillInCellScaleK(ExperimentParams experimentParams)
        {
            if (experimentParams.artificial_viscosity_skf == 2 ||
                experimentParams.average_velocity_skf == 2 ||
                experimentParams.density_skf == 2 ||
                experimentParams.int_force_skf == 2)
            {
                experimentParams.cell_scale_k = 3;
            }
            else
            {
                experimentParams.cell_scale_k = 2;
            }
        }
        string GetParamsFileName()
        {
            string filename;

            var target_particles_generator = comboBox_ParticlesGenerator.SelectedIndex;
            if (target_particles_generator == 1)
            {
                filename = "SPH2DPicGenParams.json";
            }
            else
            {
                filename = "Params.json";
            }

            return filename;
        }
        void FillInDefaultParams(ExperimentParams experimentParams)
        {
            experimentParams.dim = 2;
            experimentParams.TYPE_BOUNDARY = -2;
            experimentParams.TYPE_NON_EXISTENT = 0;
            experimentParams.TYPE_WATER = 2;
            experimentParams.enable_check_consistency = true;
            experimentParams.inf_stop = true;
            experimentParams.g = 9.8100004196167f;
            experimentParams.pi = 3.1415927410125732f;

            experimentParams.depth = 0;
            experimentParams.beach_x = 0;
            experimentParams.fluid_particles_per_d = 0;
            experimentParams.left_wall_start = 0;
            experimentParams.left_wall_end = 0;
            experimentParams.local_threads = 0;
            experimentParams.max_cells = 0;
            experimentParams.max_neighbours = 0;
            experimentParams.maxn = 0;
            experimentParams.nfluid = 0;
            experimentParams.nvirt = 0;
            experimentParams.ntotal = 0;
            experimentParams.starttimestep = 0;
            experimentParams.x_boundary_max = 0;
            experimentParams.x_boundary_min = 0;
            experimentParams.x_fluid_max = 0;
            experimentParams.x_fluid_min = 0;
            experimentParams.x_fluid_particles = 0;
            experimentParams.x_maxgeom = 0;
            experimentParams.y_maxgeom = 0;
            experimentParams.y_boundary_max = 0;
            experimentParams.y_boundary_min = 0;
            experimentParams.y_fluid_min = 0;
            experimentParams.y_fluid_max = 0;
            experimentParams.y_fluid_particles = 0;
            experimentParams.piston_amp = 0;
            experimentParams.maxtimestep = 0;
        }
        void GenerateSimulationProperies(string directory)
        {
            ExperimentParams experimentParams = new ExperimentParams();

            FillInArtificialViscosity(experimentParams);
            FillInAverageVelocity(experimentParams);
            FillInDensity(experimentParams);
            FillInInternalForce(experimentParams);
            FillInWavesGenerator(experimentParams);
            FillInGeometry(experimentParams);
            FillInBoundaries(experimentParams);
            FillInTimeStep(experimentParams);
            FillInTimeIntegration(experimentParams);
            FillInViscosity(experimentParams);
            FillInDefaultParams(experimentParams);

            FillInCellScaleK(experimentParams);

            experimentParams.experiment_name = Path.GetFileName(directory);
            experimentParams.format_line = "";
            experimentParams.version_major = 0;
            experimentParams.version_minor = 0;

            string path = Path.Combine(directory, GetParamsFileName());
            using (var stream = File.OpenWrite(path))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                JsonSerializer.Serialize(stream, experimentParams, options);
            }
        }

        void LoadGeometry(ExperimentParams experimentParams)
        {
            textBox_GeomDelta.Text = experimentParams.delta.ToString();
            textBox_GeomSmoothLen.Text = (experimentParams.hsml / experimentParams.delta).ToString();
            textBox_GeomOriginX.Text = experimentParams.x_mingeom.ToString();
            textBox_GeomOriginY.Text = experimentParams.y_mingeom.ToString();
        }
        void LoadArtificialViscosity(ExperimentParams experimentParams)
        {
            checkBox_EnableArtVisc.Checked = experimentParams.artificial_viscosity;
            comboBox_ArtViscSKF.SelectedIndex = (int)(experimentParams.artificial_viscosity_skf - 1);
            textBox_ArtViscBulk.Text = experimentParams.artificial_bulk_visc.ToString();
            textBox_ArtViscShear.Text = experimentParams.artificial_shear_visc.ToString();
        }
        void LoadAverageVelocity(ExperimentParams experimentParams)
        {
            checkBox_EnableAvVel.Checked = experimentParams.average_velocity;
            comboBox_AvVelSKF.SelectedIndex = (int)(experimentParams.average_velocity_skf - 1);
            textBox_AvVelCoef.Text = experimentParams.average_velocity_epsilon.ToString();
        }
        void LoadInternalForce(ExperimentParams experimentParams)
        {
            comboBox_IntForceSKF.SelectedIndex = (int)(experimentParams.int_force_skf - 1);
            comboBox_IntForceTreat.SelectedIndex = (int)(experimentParams.pa_sph - 1);
            textBox_IntForceSoundVelCoef.Text = experimentParams.eos_csqr_k.ToString();
        }
        void LoadDensity(ExperimentParams experimentParams)
        {
            if (experimentParams.summation_density)
            {
                comboBox_DensityTreat.SelectedIndex = 0;
            }
            else
            {
                comboBox_DensityTreat.SelectedIndex = 1;
            }

            comboBox_DensitySKF.SelectedIndex = (int)(experimentParams.density_skf - 1);
            textBox_DensityValue.Text = experimentParams.mass.ToString();
            checkBox_DensityNorm.Checked = experimentParams.nor_density;
        }
        void LoadBoundaries(ExperimentParams experimentParams)
        {
            comboBox_BoundaryTreat.SelectedIndex = (int)experimentParams.sbt;
            textBox_BoundaryDelta.Text = (experimentParams.boundary_delta / experimentParams.delta).ToString();
            textBox_BoundaryLayersNum.Text = experimentParams.boundary_layers_num.ToString();
        }
        void LoadWavesGenerator(ExperimentParams experimentParams)
        {
            checkBox_EnableWavesGen.Checked = experimentParams.waves_generator;
            if (experimentParams.nwm == 0)
            {
                checkBox_EnableWavesGen.Checked = false;
            }
            else
            {
                comboBox_WavesGenTreat.SelectedIndex = (int)(experimentParams.nwm - 1);
            }
            textBox_WavesGenFreq.Text = experimentParams.freq.ToString();
            textBox_WavesGenLen.Text = experimentParams.wave_length.ToString();
            textBox_WavesGenMagnitude.Text = experimentParams.wave_amp.ToString();
            textBox_WavesGenTimeWait.Text = experimentParams.generator_time_wait.ToString();
        }
        void LoadDynamicViscosity(ExperimentParams experimentParams)
        {
            checkBox_EnableDynVisc.Checked = experimentParams.visc;
            textBox_DynViscValue.Text = experimentParams.water_dynamic_visc.ToString();
        }
        void LoadTimeIntegration(ExperimentParams experimentParams)
        {
            comboBox_TimeTreat.SelectedIndex = (int)experimentParams.dt_correction_method;
            textBox_TimeCFL.Text = experimentParams.CFL_coef.ToString();
            textBox_TimeDT.Text = experimentParams.dt.ToString();
            textBox_TimeSim.Text = experimentParams.simulation_time.ToString();
        }
        void LoadTimeStep(ExperimentParams experimentParams)
        {
            textBox_StepSave.Text = experimentParams.save_step.ToString();
            textBox_StepDump.Text = experimentParams.dump_step.ToString();
            textBox_StepEstimate.Text = experimentParams.print_time_est_step.ToString();
            textBox_StepCheck.Text = experimentParams.normal_check_step.ToString();
        }
        void LoadTemplate(string filename)
        {
            using (var stream = File.OpenRead(filename))
            {
                var experimentParams = JsonSerializer.Deserialize<ExperimentParams>(stream);
                if (experimentParams != null)
                {
                    bool majorLess = experimentParams.version_major < TargetParamsVersionMajor;
                    bool majorEqual = experimentParams.version_major == TargetParamsVersionMajor;
                    bool minorLess = experimentParams.version_minor < TargetParamsVersionMinor;
                    if (majorLess || (majorEqual && minorLess))
                    {
                        DialogResult result = MessageBox.Show(
                            "Expected higher params version. Try to parse parameters anyway?", 
                            "Warning", 
                            MessageBoxButtons.YesNo, 
                            MessageBoxIcon.Warning);

                        if (result == DialogResult.No)
                        {
                            return;
                        }
                    }

                    LoadGeometry(experimentParams);
                    LoadArtificialViscosity(experimentParams);
                    LoadAverageVelocity(experimentParams);
                    LoadDensity(experimentParams);
                    LoadInternalForce(experimentParams);
                    LoadBoundaries(experimentParams);
                    LoadWavesGenerator(experimentParams);
                    LoadDynamicViscosity(experimentParams);
                    LoadTimeIntegration(experimentParams);
                    LoadTimeStep(experimentParams);
                }
            }
        }

        private void checkBox_EnableArtVisc_CheckedChanged(object sender, EventArgs e)
        {
            UpdateArtificialViscosity();
        }

        private void checkBox_EnableAvVel_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAverageVelocity();
        }

        private void checkBox_EnableDynVisc_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDynamicViscosity();
        }

        private void checkBox_EnableWavesGen_CheckedChanged(object sender, EventArgs e)
        {
            UpdateWavesGenerator();
        }

        private void comboBox_TimeTreat_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTimeIntegration();
        }

        private void comboBox_DensityTreat_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDensity();
        }

        private void button_GenerateProject_Click(object sender, EventArgs e)
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.InitialDirectory = LastFolderOpened ?? Directory.GetCurrentDirectory();
                dialog.IsFolderPicker = true;
                var result = dialog.ShowDialog();
                if (result == CommonFileDialogResult.Ok && !string.IsNullOrWhiteSpace(dialog.FileName))
                {
                    LastFolderOpened = dialog.FileName;
                    try
                    {
                        GenerateSimulationProperies(dialog.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error happened: " + ex.Message);
                    }
                }
            }
        }

        private void comboBox_ParticlesGenerator_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateBoundaries();
        }

        private void button_OpenAsTemplate_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.InitialDirectory = LastFolderOpened ?? Directory.GetCurrentDirectory();
                dialog.Filter = "Params file (*.json)|*.json|All files (*.*)|*.*";
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
                {
                    LastFolderOpened = Path.GetDirectoryName(dialog.FileName);
                    try
                    {
                        LoadTemplate(dialog.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error happened: " + ex.Message);
                    }
                }
            }
        }
    }
}

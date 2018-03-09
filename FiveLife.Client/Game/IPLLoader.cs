using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.Game
{
    class IPLLoader : FiveLifeScript
    {

        public override void Initialize()
        {
            // Janitors appartment
            Function.Call(Hash.REQUEST_IPL, "apa_ss1_05_interior_v_janitor_milo_");

            // Simeon: -47.16170 -1115.3327 26.5
            Function.Call(Hash.REQUEST_IPL, "shr_int");

            // Trevor: 1985.48132, 3828.76757, 32.5
            // Trash or Tidy. Only choose one.
            // Function.Call(Hash.REQUEST_IPL, "TrevorsTrailerTrash");
            Function.Call(Hash.REQUEST_IPL, "trevorstrailertidy");

            // Heist Jewel: -637.20159 -239.16250 38.1
            Function.Call(Hash.REQUEST_IPL, "post_hiest_unload");
            Function.Call(Hash.REMOVE_IPL, "jewel2fake");
            Function.Call(Hash.REMOVE_IPL, "bh1_16_refurb");

            // Max Renda: -585.8247, -282.72, 35.45475
            Function.Call(Hash.REQUEST_IPL, "refit_unload");

            // Heist Union Depository: 2.69689322, -667.0166, 16.1306286
            Function.Call(Hash.REQUEST_IPL, "FINBANK");

            // Morgue: 239.75195, -1360.64965, 39.53437
            Function.Call(Hash.REQUEST_IPL, "Coroner_Int_on");
            Function.Call(Hash.REQUEST_IPL, "coronertrash");

            // Cluckin Bell: -146.3837, 6161.5, 30.2062
            Function.Call(Hash.REMOVE_IPL, "CS1_02_cf_offmission");
            Function.Call(Hash.REQUEST_IPL, "CS1_02_cf_onmission1");
            Function.Call(Hash.REQUEST_IPL, "CS1_02_cf_onmission2");
            Function.Call(Hash.REQUEST_IPL, "CS1_02_cf_onmission3");
            Function.Call(Hash.REQUEST_IPL, "CS1_02_cf_onmission4");

            // Grapeseed's farm: 2447.9, 4973.4, 47.7
            Function.Call(Hash.REQUEST_IPL, "farm");
            Function.Call(Hash.REQUEST_IPL, "farmint");
            Function.Call(Hash.REQUEST_IPL, "farm_lod");
            Function.Call(Hash.REQUEST_IPL, "farm_props");
            Function.Call(Hash.REQUEST_IPL, "des_farmhouse");

            // FIB lobby: 105.4557, -745.4835, 44.7548
            Function.Call(Hash.REQUEST_IPL, "FIBlobby");

            // Billboard: iFruit
            Function.Call(Hash.REQUEST_IPL, "FruitBB");
            Function.Call(Hash.REQUEST_IPL, "sc1_01_newbill");
            Function.Call(Hash.REQUEST_IPL, "hw1_02_newbill");
            Function.Call(Hash.REQUEST_IPL, "hw1_emissive_newbill");
            Function.Call(Hash.REQUEST_IPL, "sc1_14_newbill");
            Function.Call(Hash.REQUEST_IPL, "dt1_17_newbill");

            // Lester's factory: 716.84, -962.05, 31.59
            Function.Call(Hash.REQUEST_IPL, "id2_14_during_door");
            Function.Call(Hash.REQUEST_IPL, "id2_14_during1");

            // Life Invader lobby: -1047.9, -233.0, 39.0
            Function.Call(Hash.REQUEST_IPL, "facelobby");

            // Tunnels
            Function.Call(Hash.REQUEST_IPL, "v_tunnel_hole");

            // Carwash: 55.7, -1391.3, 30.5
            Function.Call(Hash.REQUEST_IPL, "Carwash_with_spinners");

            // Stadium "Fame or Shame": -248.49159240722656, -2010.509033203125, 34.57429885864258
            Function.Call(Hash.REQUEST_IPL, "sp1_10_real_interior");
            Function.Call(Hash.REQUEST_IPL, "sp1_10_real_interior_lod");

            // House in Banham Canyon: -3086.428, 339.2523, 6.3717
            Function.Call(Hash.REQUEST_IPL, "ch1_02_open");

            // Garage in La Mesa (autoshop): 970.27453, -1826.56982, 31.11477
            Function.Call(Hash.REQUEST_IPL, "bkr_bi_id1_23_door");

            // Hill Valley church - Grave: -282.46380000, 2835.84500000, 55.91446000
            Function.Call(Hash.REQUEST_IPL, "lr_cs6_08_grave_closed");

            // Lost's trailer park: 49.49379000, 3744.47200000, 46.38629000
            Function.Call(Hash.REQUEST_IPL, "methtrailer_grp1");

            // Lost safehouse: 984.1552, -95.3662, 74.50
            Function.Call(Hash.REQUEST_IPL, "bkr_bi_hw1_13_int");

            // Raton Canyon river: -1652.83, 4445.28, 2.52
            Function.Call(Hash.REQUEST_IPL, "CanyonRvrShallow");

            // Zancudo Gates (GTAO like): -1600.30100000, 2806.73100000, 18.79683000
            Function.Call(Hash.REQUEST_IPL, "CS3_07_MPGates");

            // Pillbox hospital:
            Function.Call(Hash.REQUEST_IPL, "rc12b_default");
            Function.Call(Hash.REQUEST_IPL, "RC12B_HospitalInterior");

            // Josh's house: -1117.1632080078, 303.090698, 66.52217
            Function.Call(Hash.REQUEST_IPL, "bh1_47_joshhse_unburnt");
            Function.Call(Hash.REQUEST_IPL, "bh1_47_joshhse_unburnt_lod");

            // Zancudo River (need streamed content): 86.815, 3191.649, 30.463
            Function.Call(Hash.REQUEST_IPL, "cs3_05_water_grp1");
            Function.Call(Hash.REQUEST_IPL, "cs3_05_water_grp1_lod");
            Function.Call(Hash.REQUEST_IPL, "cs3_05_water_grp2");
            Function.Call(Hash.REQUEST_IPL, "cs3_05_water_grp2_lod");

            // Cassidy Creek (need streamed content): -425.677, 4433.404, 27.3253
            Function.Call(Hash.REQUEST_IPL, "canyonriver01");
            Function.Call(Hash.REQUEST_IPL, "canyonriver01_lod");

            // Optional
            // Graffitis
            Function.Call(Hash.REQUEST_IPL, "ch3_rd2_bishopschickengraffiti"); // 1861.28, 2402.11, 58.53
            Function.Call(Hash.REQUEST_IPL, "cs5_04_mazebillboardgraffiti"); // 2697.32, 3162.18, 58.1
            Function.Call(Hash.REQUEST_IPL, "cs5_roads_ronoilgraffiti"); // 2119.12, 3058.21, 53.25

            // Heist Carrier: 3082.3117 -4717.1191 15.2622
            //Function.Call(Hash.REQUEST_IPL, "hei_carrier");
            //Function.Call(Hash.REQUEST_IPL, "hei_carrier_distantlights");
            //Function.Call(Hash.REQUEST_IPL, "hei_Carrier_int1");
            //Function.Call(Hash.REQUEST_IPL, "hei_Carrier_int2");
            //Function.Call(Hash.REQUEST_IPL, "hei_Carrier_int3");
            //Function.Call(Hash.REQUEST_IPL, "hei_Carrier_int4");
            //Function.Call(Hash.REQUEST_IPL, "hei_Carrier_int5");
            //Function.Call(Hash.REQUEST_IPL, "hei_Carrier_int6");
            //Function.Call(Hash.REQUEST_IPL, "hei_carrier_lodlights");
            //Function.Call(Hash.REQUEST_IPL, "hei_carrier_slod");

            // Heist Yatch: -2043.974,-1031.582, 11.981
            Function.Call(Hash.REQUEST_IPL, "hei_yacht_heist");
            Function.Call(Hash.REQUEST_IPL, "hei_yacht_heist_Bar");
            Function.Call(Hash.REQUEST_IPL, "hei_yacht_heist_Bedrm");
            Function.Call(Hash.REQUEST_IPL, "hei_yacht_heist_Bridge");
            Function.Call(Hash.REQUEST_IPL, "hei_yacht_heist_DistantLights");
            Function.Call(Hash.REQUEST_IPL, "hei_yacht_heist_enginrm");
            Function.Call(Hash.REQUEST_IPL, "hei_yacht_heist_LODLights");
            Function.Call(Hash.REQUEST_IPL, "hei_yacht_heist_Lounge");

            // Bunkers - Exteriors
            //Function.Call(Hash.REQUEST_IPL, "gr_case0_bunkerclosed");
            //Function.Call(Hash.REQUEST_IPL, "gr_case1_bunkerclosed");
            //Function.Call(Hash.REQUEST_IPL, "gr_case2_bunkerclosed");
            //Function.Call(Hash.REQUEST_IPL, "gr_case3_bunkerclosed");
            //Function.Call(Hash.REQUEST_IPL, "gr_case4_bunkerclosed");
            //Function.Call(Hash.REQUEST_IPL, "gr_case5_bunkerclosed");
            //Function.Call(Hash.REQUEST_IPL, "gr_case6_bunkerclosed");
            //Function.Call(Hash.REQUEST_IPL, "gr_case7_bunkerclosed");
            //Function.Call(Hash.REQUEST_IPL, "gr_case9_bunkerclosed");
            //Function.Call(Hash.REQUEST_IPL, "gr_case10_bunkerclosed");
            //Function.Call(Hash.REQUEST_IPL, "gr_case11_bunkerclosed");

            // Bunkers - Interior: 892.6384, -3245.8664, -98.2645
            //Function.Call(Hash.REQUEST_IPL, "gr_entrance_placement");
            //Function.Call(Hash.REQUEST_IPL, "gr_grdlc_interior_placement");
            //Function.Call(Hash.REQUEST_IPL, "gr_grdlc_interior_placement_interior_0_grdlc_int_01_milo_");
            //Function.Call(Hash.REQUEST_IPL, "gr_grdlc_interior_placement_interior_1_grdlc_int_02_milo_");

            // Bahama Mamas: -1388.0013, -618.41967, 30.819599
            // Function.Call(Hash.REQUEST_IPL, "hei_sm_16_interior_v_bahama_milo_");

            // Red Carpet: 300.5927, 199.7589, 104.3776
            // Function.Call(Hash.REMOVE_IPL, "redCarpet");

            // UFO
            // Zancudo: -2051.99463, 3237.05835, 1456.97021
            // Hippie base: 2490.47729, 3774.84351, 2414.035
            // Chiliad: 501.52880000, 5593.86500000, 796.23250000
            // Function.Call(Hash.REQUEST_IPL, "ufo");
            // Function.Call(Hash.REQUEST_IPL, "ufo_eye");
            // Function.Call(Hash.REQUEST_IPL, "ufo_lod");

            // North Yankton: 3217.697, -4834.826, 111.8152
            // Function.Call(Hash.REQUEST_IPL, "prologue01");
            // Function.Call(Hash.REQUEST_IPL, "prologue01c");
            // Function.Call(Hash.REQUEST_IPL, "prologue01d");
            // Function.Call(Hash.REQUEST_IPL, "prologue01e");
            // Function.Call(Hash.REQUEST_IPL, "prologue01f");
            // Function.Call(Hash.REQUEST_IPL, "prologue01g");
            // Function.Call(Hash.REQUEST_IPL, "prologue01h");
            // Function.Call(Hash.REQUEST_IPL, "prologue01i");
            // Function.Call(Hash.REQUEST_IPL, "prologue01j");
            // Function.Call(Hash.REQUEST_IPL, "prologue01k");
            // Function.Call(Hash.REQUEST_IPL, "prologue01z");
            // Function.Call(Hash.REQUEST_IPL, "prologue02");
            // Function.Call(Hash.REQUEST_IPL, "prologue03");
            // Function.Call(Hash.REQUEST_IPL, "prologue03b");
            // Function.Call(Hash.REQUEST_IPL, "prologue04");
            // Function.Call(Hash.REQUEST_IPL, "prologue04b");
            // Function.Call(Hash.REQUEST_IPL, "prologue05");
            // Function.Call(Hash.REQUEST_IPL, "prologue05b");
            // Function.Call(Hash.REQUEST_IPL, "prologue06");
            // Function.Call(Hash.REQUEST_IPL, "prologue06b");
            // Function.Call(Hash.REQUEST_IPL, "prologue06_int");
            // Function.Call(Hash.REQUEST_IPL, "prologuerd");
            // Function.Call(Hash.REQUEST_IPL, "prologuerdb ");
            // Function.Call(Hash.REQUEST_IPL, "prologue_DistantLights");
            // Function.Call(Hash.REQUEST_IPL, "prologue_LODLights");
            // Function.Call(Hash.REQUEST_IPL, "prologue_m2_door");

            // CEO Offices :
            // Arcadius Business Centre
            Function.Call(Hash.REMOVE_IPL, "ex_dt1_02_office_02b");   // Executive Rich
            Function.Call(Hash.REMOVE_IPL, "ex_dt1_02_office_02c");    // Executive Cool
            Function.Call(Hash.REMOVE_IPL, "ex_dt1_02_office_02a");    // Executive Contrast
            Function.Call(Hash.REMOVE_IPL, "ex_dt1_02_office_01a");    // Old Spice Warm
            Function.Call(Hash.REMOVE_IPL, "ex_dt1_02_office_01b");    // Old Spice Classical
            Function.Call(Hash.REMOVE_IPL, "ex_dt1_02_office_01c");    // Old Spice Vintage
            Function.Call(Hash.REMOVE_IPL, "ex_dt1_02_office_03a");    // Power Broker Ice
            Function.Call(Hash.REMOVE_IPL, "ex_dt1_02_office_03b");    // Power Broker Conservative
            Function.Call(Hash.REMOVE_IPL, "ex_dt1_02_office_03c");    // Power Broker Polished

            // Maze Bank Building
            Function.Call(Hash.REMOVE_IPL, "ex_dt1_11_office_02b");    // Executive Rich
            Function.Call(Hash.REMOVE_IPL, "ex_dt1_11_office_02c");    // Executive Cool
            Function.Call(Hash.REMOVE_IPL, "ex_dt1_11_office_02a");    // Executive Contrast
            Function.Call(Hash.REMOVE_IPL, "ex_dt1_11_office_01a");    // Old Spice Warm
            Function.Call(Hash.REMOVE_IPL, "ex_dt1_11_office_01b");    // Old Spice Classical
            Function.Call(Hash.REMOVE_IPL, "ex_dt1_11_office_01c");    // Old Spice Vintage
            Function.Call(Hash.REMOVE_IPL, "ex_dt1_11_office_03a");    // Power Broker Ice
            Function.Call(Hash.REMOVE_IPL, "ex_dt1_11_office_03b");    // Power Broker Conservative
            Function.Call(Hash.REMOVE_IPL, "ex_dt1_11_office_03c");    // Power Broker Polished

            // Lom Bank
            Function.Call(Hash.REMOVE_IPL, "ex_sm_13_office_02b"); // Executive Rich
            Function.Call(Hash.REMOVE_IPL, "ex_sm_13_office_02c"); // Executive Cool
            Function.Call(Hash.REMOVE_IPL, "ex_sm_13_office_02a"); // Executive Contrast
            Function.Call(Hash.REMOVE_IPL, "ex_sm_13_office_01a"); // Old Spice Warm
            Function.Call(Hash.REMOVE_IPL, "ex_sm_13_office_01b"); // Old Spice Classical
            Function.Call(Hash.REMOVE_IPL, "ex_sm_13_office_01c"); // Old Spice Vintage
            Function.Call(Hash.REMOVE_IPL, "ex_sm_13_office_03a"); // Power Broker Ice
            Function.Call(Hash.REMOVE_IPL, "ex_sm_13_office_03b"); // Power Broker Conservative
            Function.Call(Hash.REMOVE_IPL, "ex_sm_13_office_03c");  // Power Broker Polished

            // Maze Bank West
            Function.Call(Hash.REMOVE_IPL, "ex_sm_15_office_02b");  // Executive Rich
            Function.Call(Hash.REMOVE_IPL, "ex_sm_15_office_02c");  // Executive Cool
            Function.Call(Hash.REMOVE_IPL, "ex_sm_15_office_02a");  // Executive Contrast
            Function.Call(Hash.REMOVE_IPL, "ex_sm_15_office_01a");  // Old Spice Warm
            Function.Call(Hash.REMOVE_IPL, "ex_sm_15_office_01b");  // Old Spice Classical
            Function.Call(Hash.REMOVE_IPL, "ex_sm_15_office_01c");  // Old Spice Vintage
            Function.Call(Hash.REMOVE_IPL, "ex_sm_15_office_03a");  // Power Broker Ice
            Function.Call(Hash.REMOVE_IPL, "ex_sm_15_office_03b");  // Power Broker Convservative
            Function.Call(Hash.REMOVE_IPL, "ex_sm_15_office_03c");	// Power Broker Polished

            // Biker
            Function.Call(Hash.REMOVE_IPL, "bkr_biker_interior_placement_interior_0_biker_dlc_int_01_milo");
            Function.Call(Hash.REMOVE_IPL, "bkr_biker_interior_placement_interior_1_biker_dlc_int_02_milo");

            Function.Call(Hash.REMOVE_IPL, "bkr_biker_interior_placement_interior_2_biker_dlc_int_ware01_milo");
            Function.Call(Hash.REMOVE_IPL, "bkr_biker_interior_placement_interior_2_biker_dlc_int_ware02_milo");
            Function.Call(Hash.REMOVE_IPL, "bkr_biker_interior_placement_interior_2_biker_dlc_int_ware03_milo");
            Function.Call(Hash.REMOVE_IPL, "bkr_biker_interior_placement_interior_2_biker_dlc_int_ware04_milo");
            Function.Call(Hash.REMOVE_IPL, "bkr_biker_interior_placement_interior_2_biker_dlc_int_ware05_milo");

            Function.Call(Hash.REMOVE_IPL, "ex_exec_warehouse_placement_interior_0_int_warehouse_m_dlc_milo ");
            Function.Call(Hash.REMOVE_IPL, "ex_exec_warehouse_placement_interior_1_int_warehouse_s_dlc_milo ");
            Function.Call(Hash.REMOVE_IPL, "ex_exec_warehouse_placement_interior_2_int_warehouse_l_dlc_milo ");

            // IMPORT/EXPORT
            Function.Call(Hash.REMOVE_IPL, "imp_dt1_02_modgarage");
            Function.Call(Hash.REMOVE_IPL, "imp_dt1_02_cargarage_a");
            Function.Call(Hash.REMOVE_IPL, "imp_dt1_02_cargarage_b");
            Function.Call(Hash.REMOVE_IPL, "imp_dt1_02_cargarage_c");

            Function.Call(Hash.REMOVE_IPL, "imp_dt1_11_modgarage");
            Function.Call(Hash.REMOVE_IPL, "imp_dt1_11_cargarage_a");
            Function.Call(Hash.REMOVE_IPL, "imp_dt1_11_cargarage_b");
            Function.Call(Hash.REMOVE_IPL, "imp_dt1_11_cargarage_c");

            Function.Call(Hash.REMOVE_IPL, "imp_sm_13_modgarage");
            Function.Call(Hash.REMOVE_IPL, "imp_sm_13_cargarage_a");
            Function.Call(Hash.REMOVE_IPL, "imp_sm_13_cargarage_b");
            Function.Call(Hash.REMOVE_IPL, "imp_sm_13_cargarage_c");

            Function.Call(Hash.REMOVE_IPL, "imp_sm_15_modgarage");
            Function.Call(Hash.REMOVE_IPL, "imp_sm_15_cargarage_a");
            Function.Call(Hash.REMOVE_IPL, "imp_sm_15_cargarage_b");
            Function.Call(Hash.REMOVE_IPL, "imp_sm_15_cargarage_c");

            Function.Call(Hash.REMOVE_IPL, "imp_impexp_interior_placement");
            Function.Call(Hash.REMOVE_IPL, "imp_impexp_interior_placement_interior_0_impexp_int_01_milo_");
            Function.Call(Hash.REMOVE_IPL, "imp_impexp_interior_placement_interior_3_impexp_int_02_milo_");
            Function.Call(Hash.REMOVE_IPL, "imp_impexp_interior_placement_interior_1_impexp_intwaremed_milo_");
            Function.Call(Hash.REMOVE_IPL, "imp_impexp_interior_placement_interior_2_imptexp_mod_int_01_milo_");

            // Online Appartments
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_01_a");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_01_b");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_01_c");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_02_a");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_02_b");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_02_c");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_03_a");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_03_b");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_03_c");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_04_a");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_04_b");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_04_c");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_05_a");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_05_b");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_05_c");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_06_a");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_06_b");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_06_c");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_07_a");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_07_b");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_07_c");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_08_a");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_08_b");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_08_c");


            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_01_a");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_01_b");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_01_c");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_02_a");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_02_b");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_02_c");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_03_a");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_03_b");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_03_c");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_04_a");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_04_b");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_04_c");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_05_a");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_05_b");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_05_c");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_06_a");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_06_b");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_06_c");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_07_a");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_07_b");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_07_c");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_08_a");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_08_b");
            Function.Call(Hash.REMOVE_IPL, "apa_v_mp_h_08_c");

            // Lab
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_33_x17dlc_int_02_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_34_x17dlc_int_lab_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_35_x17dlc_int_tun_entry_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_4_x17dlc_int_facility_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_5_x17dlc_int_facility2_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_9_x17dlc_int_01_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_0_x17dlc_int_base_ent_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_1_x17dlc_int_base_loop_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_10_x17dlc_int_tun_straight_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_11_x17dlc_int_tun_slope_flat_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_12_x17dlc_int_tun_flat_slope_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_13_x17dlc_int_tun_30d_r_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_14_x17dlc_int_tun_30d_l_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_15_x17dlc_int_tun_straight_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_16_x17dlc_int_tun_straight_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_17_x17dlc_int_tun_slope_flat_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_18_x17dlc_int_tun_slope_flat_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_19_x17dlc_int_tun_flat_slope_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_2_x17dlc_int_bse_tun_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_20_x17dlc_int_tun_flat_slope_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_21_x17dlc_int_tun_30d_r_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_22_x17dlc_int_tun_30d_r_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_23_x17dlc_int_tun_30d_r_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_24_x17dlc_int_tun_30d_r_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_25_x17dlc_int_tun_30d_l_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_26_x17dlc_int_tun_30d_l_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_27_x17dlc_int_tun_30d_l_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_28_x17dlc_int_tun_30d_l_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_29_x17dlc_int_tun_30d_l_milo_");
            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_3_x17dlc_int_base_milo_");

            Function.Call(Hash.REMOVE_IPL, "xm_x17dlc_int_placement_interior_7_x17dlc_int_silo_02_milo_");

        }

    }
}

using System.Collections.Generic;
using UnityEngine;

namespace NullSave
{
    [DefaultExecutionOrder(-100)]
    [RequireComponent(typeof(Rewired.InputManager))]
    public class ReIconed : MonoBehaviour
    {

        #region Variables

        // List of associated maps
        public List<ControllerMap> controllerMaps;

        // Map lists
        private static List<Rewired.ActionElementMap> axisMaps;
        private static List<Rewired.ActionElementMap> buttonMaps;

        #endregion

        #region Properties

        // Instance of ReIconed
        public static ReIconed Instance;

        #endregion

        #region Unity Methods

        // Instance and create lists
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                axisMaps = new List<Rewired.ActionElementMap>();
                buttonMaps = new List<Rewired.ActionElementMap>();
            }
        }

        #endregion

        #region Public Methods

        public static InputMap GetActionHardwareInput(string action)
        {
            return GetActionHardwareInput(action, 0);
        }

        public static InputMap GetActionHardwareInput(int actionId)
        {
            return GetActionHardwareInput(actionId, 0);
        }

        public static InputMap GetActionHardwareInput(string action, int playerId)
        {
            if (Rewired.ReInput.players == null || Rewired.ReInput.players.playerCount <= 0) return null;

            Rewired.Player player = Rewired.ReInput.players.GetPlayer(playerId);
            ReIconedModifiers mod = ActionModifiers(ref action);
            int actionId = Rewired.ReInput.mapping.GetActionId(action);

            return GetActionHardwareInput(actionId, player, mod);
        }

        public static InputMap GetActionHardwareInput(int actionId, int playerId)
        {
            if (Rewired.ReInput.players == null || Rewired.ReInput.players.playerCount <= 0) return null;

            Rewired.Player player = Rewired.ReInput.players.GetPlayer(playerId);

            return GetActionHardwareInput(actionId, player, ReIconedModifiers.None);
        }

        #endregion

        #region Private Methods

        private static ReIconedModifiers ActionModifiers(ref string action)
        {
            ReIconedModifiers modifiers = ReIconedModifiers.None;

            if (action[action.Length - 1] == '+')
            {
                action = action.Substring(0, action.Length - 1);
                modifiers = ReIconedModifiers.Positive;
            }
            else if (action[action.Length - 1] == '-')
            {
                action = action.Substring(0, action.Length - 1);
                modifiers = ReIconedModifiers.Negative;
            }
            else if (action[action.Length - 1] == '/')
            {
                action = action.Substring(0, action.Length - 1);
                modifiers = ReIconedModifiers.Dual;
            }
            else if (action[action.Length - 1] == '*')
            {
                action = action.Substring(0, action.Length - 1);
                modifiers = ReIconedModifiers.All;
            }

            return modifiers;
        }

        private static string SelectMapName(Rewired.ActionElementMap map, ReIconedModifiers modifiers)
        {
            switch (modifiers)
            {
                case ReIconedModifiers.All:
                    return map.elementIdentifierName + "*";
                case ReIconedModifiers.Dual:
                    return map.elementIdentifierName + "/";
                case ReIconedModifiers.Negative:
                    return map.elementIdentifierName + "-";
                case ReIconedModifiers.Positive:
                    return map.elementIdentifierName + "+";
                default:
                    return map.elementIdentifierName;
            }
        }

        private InputMap GetActionInput(ControllerMap map, string inputName)
        {
            foreach (InputMap input in map.inputMaps)
            {
                if (input.inputName == inputName)
                {
                    input.TMPSpriteAsset = map.tmpSpriteAsset;
                    return input;
                }
            }

            if (inputName[inputName.Length - 1] == '+' || inputName[inputName.Length - 1] == '-' || inputName[inputName.Length - 1] == '/' || inputName[inputName.Length - 1] == '*')
            {
                inputName = inputName.Substring(0, inputName.Length - 1);
                foreach (InputMap input in map.inputMaps)
                {
                    if (input.inputName == inputName)
                    {
                        input.TMPSpriteAsset = map.tmpSpriteAsset;
                        return input;
                    }
                }
            }

            return null;
        }

        private static InputMap GetActionHardwareInput(int actionId, Rewired.Player player, ReIconedModifiers modifiers)
        {
            InputMap result = null;

            if (player != null)
            {
                // Get Maps
                player.controllers.maps.GetAxisMapsWithAction(actionId, true, axisMaps);
                player.controllers.maps.GetButtonMapsWithAction(actionId, true, buttonMaps);

                if (axisMaps.Count > 0)
                {
                    result = GetActionMapJoystick(axisMaps, player, modifiers);
                    if (result != null) return result;
                }

                if (buttonMaps.Count > 0)
                {
                    result = GetActionMapJoystick(buttonMaps, player, modifiers);
                    if (result != null) return result;
                }

                if (axisMaps.Count > 0)
                {
                    result = GetActionMapKeyboard(axisMaps, player, modifiers);
                    if (result != null) return result;
                }

                if (buttonMaps.Count > 0)
                {
                    result = GetActionMapKeyboard(buttonMaps, player, modifiers);
                    if (result != null) return result;
                }
            }

            return result;
        }

        private static InputMap GetActionMapJoystick(List<Rewired.ActionElementMap> maps, Rewired.Player player, ReIconedModifiers mod)
        {
            foreach (Rewired.ActionElementMap map in maps)
            {
                if (map.enabled)
                {
                    foreach (Rewired.Controller controller in player.controllers.Joysticks)
                    {
                        if (controller.enabled && controller.isConnected && (controller.type == Rewired.ControllerType.Custom || controller.type == Rewired.ControllerType.Joystick))
                        {
                            foreach (Rewired.ControllerElementIdentifier element in controller.ElementIdentifiers)
                            {
                                if (element.name == map.elementIdentifierName)
                                {
                                    return Instance.GetMapping(controller.hardwareTypeGuid.ToString(), SelectMapName(map, mod));
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        private static InputMap GetActionMapKeyboard(List<Rewired.ActionElementMap> maps, Rewired.Player player, ReIconedModifiers mod)
        {
            foreach (Rewired.ActionElementMap map in maps)
            {
                if (map.enabled && MapMatchesModifiers(map, mod))
                {
                    foreach (Rewired.Controller controller in player.controllers.Controllers)
                    {
                        if (controller.enabled && controller.isConnected && (controller.type == Rewired.ControllerType.Keyboard || controller.type == Rewired.ControllerType.Mouse))
                        {
                            foreach (Rewired.ControllerElementIdentifier element in controller.ElementIdentifiers)
                            {
                                if (element.name == map.elementIdentifierName)
                                {
                                    return Instance.GetMapping("Keyboard", SelectMapName(map, mod));
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        private InputMap GetMapping(string hardwareId, string inputName)
        {
            ControllerMap defaultMap = null;
            InputMap actionInput;

            foreach (ControllerMap map in controllerMaps)
            {
                if (map.compatibleDevices.Contains(hardwareId))
                {
                    actionInput = GetActionInput(map, inputName);
                    if (actionInput != null) return actionInput;
                }

                if (map.isFallback)
                {
                    defaultMap = map;
                }
            }

            if (defaultMap != null)
            {
                return GetActionInput(defaultMap, inputName);
            }

            return null;
        }

        private static bool MapMatchesModifiers(Rewired.ActionElementMap map, ReIconedModifiers mod)
        {
            switch (mod)
            {
                case ReIconedModifiers.Negative:
                    return map.axisContribution == Rewired.Pole.Negative;
                case ReIconedModifiers.Positive:
                    return map.axisContribution == Rewired.Pole.Positive;
                default:
                    return true;
            }
        }

        #endregion

    }
}
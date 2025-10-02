using System;

namespace Unity.Cloud.AssetsEmbedded
{
    static class FieldDefinitionExtensions
    {
        /// <summary>
        /// Returns the field definition as a <see cref="ISelectionFieldDefinition"/>.
        /// </summary>
        /// <param name="fieldDefinition">A field definition. </param>
        /// <returns>A <see cref="ISelectionFieldDefinition"/>. </returns>
        /// <exception cref="InvalidCastException">If the field definition is not of type <see cref="FieldDefinitionType.Selection"/></exception>
        public static ISelectionFieldDefinition AsSelectionFieldDefinition(this IFieldDefinition fieldDefinition)
        {
            if (fieldDefinition is ISelectionFieldDefinition selectionFieldDefinition)
            {
                return selectionFieldDefinition;
            }

            if (fieldDefinition is FieldDefinitionEntity fieldDefinitionEntity)
            {
                return fieldDefinitionEntity.AsSelectionFieldDefinitionEntity();
            }

            throw new InvalidCastException("Field definition is not a selection field definition.");
        }

        /// <summary>
        /// Returns the field definition properties for a field of type selection.
        /// </summary>
        /// <param name="fieldDefinitionProperties">A field definition's properties. </param>
        /// <returns>A <see cref="SelectionFieldDefinitionProperties"/>. </returns>
        /// <exception cref="InvalidCastException">If the field definition is not of type <see cref="FieldDefinitionType.Selection"/></exception>
        public static SelectionFieldDefinitionProperties AsSelectionFieldDefinitionProperties(this FieldDefinitionProperties fieldDefinitionProperties)
        {
            if (fieldDefinitionProperties.Type == FieldDefinitionType.Selection)
            {
                return fieldDefinitionProperties.From();
            }

            throw new InvalidCastException("Field definition properties are not of type selection.");
        }
    }
}

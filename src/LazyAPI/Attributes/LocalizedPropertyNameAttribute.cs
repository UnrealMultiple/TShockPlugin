/**
 * Copyright (c) 2007 James Newton-King
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using Newtonsoft.Json;

namespace LazyAPI.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class LocalizedPropertyNameAttribute : Attribute
{
    public string Text { get; }

    public string Type { get; }


    internal NullValueHandling? _nullValueHandling;
    internal DefaultValueHandling? _defaultValueHandling;
    internal ReferenceLoopHandling? _referenceLoopHandling;
    internal ObjectCreationHandling? _objectCreationHandling;
    internal TypeNameHandling? _typeNameHandling;
    internal bool? _isReference;
    internal int? _order;
    internal Required? _required;
    internal bool? _itemIsReference;
    internal ReferenceLoopHandling? _itemReferenceLoopHandling;
    internal TypeNameHandling? _itemTypeNameHandling;

    /// <summary>
    /// Gets or sets the <see cref="JsonConverter"/> type used when serializing the property's collection items.
    /// </summary>
    /// <value>The collection's items <see cref="JsonConverter"/> type.</value>
    public Type? ItemConverterType { get; set; }

    /// <summary>
    /// The parameter list to use when constructing the <see cref="JsonConverter"/> described by <see cref="ItemConverterType"/>.
    /// If <c>null</c>, the default constructor is used.
    /// When non-<c>null</c>, there must be a constructor defined in the <see cref="JsonConverter"/> that exactly matches the number,
    /// order, and type of these parameters.
    /// </summary>
    /// <example>
    /// <code>
    /// [JsonProperty(ItemConverterType = typeof(MyContainerConverter), ItemConverterParameters = new object[] { 123, "Four" })]
    /// </code>
    /// </example>
    public object[]? ItemConverterParameters { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Type"/> of the <see cref="NamingStrategy"/>.
    /// </summary>
    /// <value>The <see cref="Type"/> of the <see cref="NamingStrategy"/>.</value>
    public Type? NamingStrategyType { get; set; }

    /// <summary>
    /// The parameter list to use when constructing the <see cref="NamingStrategy"/> described by <see cref="JsonPropertyAttribute.NamingStrategyType"/>.
    /// If <c>null</c>, the default constructor is used.
    /// When non-<c>null</c>, there must be a constructor defined in the <see cref="NamingStrategy"/> that exactly matches the number,
    /// order, and type of these parameters.
    /// </summary>
    /// <example>
    /// <code>
    /// [JsonProperty(NamingStrategyType = typeof(MyNamingStrategy), NamingStrategyParameters = new object[] { 123, "Four" })]
    /// </code>
    /// </example>
    public object[]? NamingStrategyParameters { get; set; }

    /// <summary>
    /// Gets or sets the null value handling used when serializing this property.
    /// </summary>
    /// <value>The null value handling.</value>
    public NullValueHandling NullValueHandling
    {
        get => this._nullValueHandling ?? default;
        set => this._nullValueHandling = value;
    }

    /// <summary>
    /// Gets or sets the default value handling used when serializing this property.
    /// </summary>
    /// <value>The default value handling.</value>
    public DefaultValueHandling DefaultValueHandling
    {
        get => this._defaultValueHandling ?? default;
        set => this._defaultValueHandling = value;
    }

    /// <summary>
    /// Gets or sets the reference loop handling used when serializing this property.
    /// </summary>
    /// <value>The reference loop handling.</value>
    public ReferenceLoopHandling ReferenceLoopHandling
    {
        get => this._referenceLoopHandling ?? default;
        set => this._referenceLoopHandling = value;
    }

    /// <summary>
    /// Gets or sets the object creation handling used when deserializing this property.
    /// </summary>
    /// <value>The object creation handling.</value>
    public ObjectCreationHandling ObjectCreationHandling
    {
        get => this._objectCreationHandling ?? default;
        set => this._objectCreationHandling = value;
    }

    /// <summary>
    /// Gets or sets the type name handling used when serializing this property.
    /// </summary>
    /// <value>The type name handling.</value>
    public TypeNameHandling TypeNameHandling
    {
        get => this._typeNameHandling ?? default;
        set => this._typeNameHandling = value;
    }

    /// <summary>
    /// Gets or sets whether this property's value is serialized as a reference.
    /// </summary>
    /// <value>Whether this property's value is serialized as a reference.</value>
    public bool IsReference
    {
        get => this._isReference ?? default;
        set => this._isReference = value;
    }

    /// <summary>
    /// Gets or sets the order of serialization of a member.
    /// </summary>
    /// <value>The numeric order of serialization.</value>
    public int Order
    {
        get => this._order ?? default;
        set => this._order = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this property is required.
    /// </summary>
    /// <value>
    /// 	A value indicating whether this property is required.
    /// </value>
    public Required Required
    {
        get => this._required ?? Required.Default;
        set => this._required = value;
    }

    /// <summary>
    /// Gets or sets the reference loop handling used when serializing the property's collection items.
    /// </summary>
    /// <value>The collection's items reference loop handling.</value>
    public ReferenceLoopHandling ItemReferenceLoopHandling
    {
        get => this._itemReferenceLoopHandling ?? default;
        set => this._itemReferenceLoopHandling = value;
    }

    /// <summary>
    /// Gets or sets the type name handling used when serializing the property's collection items.
    /// </summary>
    /// <value>The collection's items type name handling.</value>
    public TypeNameHandling ItemTypeNameHandling
    {
        get => this._itemTypeNameHandling ?? default;
        set => this._itemTypeNameHandling = value;
    }

    /// <summary>
    /// Gets or sets whether this property's collection items are serialized as a reference.
    /// </summary>
    /// <value>Whether this property's collection items are serialized as a reference.</value>
    public bool ItemIsReference
    {
        get => this._itemIsReference ?? default;
        set => this._itemIsReference = value;
    }

    public LocalizedPropertyNameAttribute(string type, string text)
    {
        this.Text = text;
        this.Type = type;
    }
}

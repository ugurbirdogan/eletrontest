using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using YemekSepeti.Service.YSServiceReference;

namespace YemekSepeti.Service
{
    /*
       * GetMessage():
       * GetMessage is used in order to get authenticated restaurant’s message. 
       * It takes no parameters and returns an XML String. Returns null if there is 
       * no new message or authentication failed.
       * Message string has three nodes. Order, products and options.
       * 
       * Orders:
       * Attribute Name 	Description
       * Id 				Yemeksepeti order identifier.
       * MessageId 			Message’s identifier.
       * RestaurantName 	Restaurant’s display name.
       * RestaurantCatalog 	Catalog Name of the restaurant.
       * RestaurantCategory Category Name of the restaurant.
       * CustomerName 		Customer’s full name.
       * CustomerId 		Yemeksepeti customer identifier.
       * CustomerType 		Customer’s type. If 1 it is a new customer.
       * PaymentNote 		Additional note for the payment.
       * OrderTotal 		Order’s total.
       * CustomerPhone 		Customer’s phone number.
       * City 				Customer’s city.
       * Region 			Customer’s region name.
       * Organization 		Customer’s organization.
       * Address 			Customer’s address.
       * AddressDescription Customer’s address’ description.
       * AddressId 			Address identifier
       * PaymentMethodId 	Order’s payment type identifier.
       * DeliveryTime 		Order’s delivery time.
       * ChangeInTotal 		If there’s a discount or some additional fees, this will be reflected in here.
       * Currency 			Order’s preffered currency.
       * 
       * Products:
       * Attribute Name 	Description
       * Id 				Yemeksepeti product identifier.
       * Name 				Product’s name.
       * Price 				Product’s unit price.
       * Quantity 			Product’s quantity.
       * Options 			Product’s additional options.
       * OptionIds 			Product’s additional option identifiers.
       * OrderIndex 		Product’s index on order.
       * ParentIndex 		If product has a parent on this order, this attribute shows the index of the main product.
       * PromoParentIndex 	If product is a promotion for another product, this attribute shows the index of the main product.
       * ProductOptionId 	If product is an option for another product, it shows the option identifier.
	   
       * Options: 
       * Attribute Name 	Description
       * Id 				Yemeksepeti option identifier.
       * Name 				Option’s name.
       * ProductOrderId 	Product’s index on order.
     */

    public partial class YemekSepetiClient
    {
        public string GetMessage()
        {
            GetMessageRequest getMessageRequest = new GetMessageRequest(Auth);
            var response = client.GetMessage(getMessageRequest).GetMessageResult;

            //XML Serialization ...
            return response;
        }


        public string GetAllMessages()
        {
            GetAllMessagesRequest getAllMessagesRequest = new GetAllMessagesRequest(Auth);
            var response = client.GetAllMessages(getAllMessagesRequest).GetAllMessagesResult;
            // DbSet .NEt -> Restaurant, Menu
            // XML -> Messages (Orders' and Options)
            if (response == null) return null; // !!!!!! VOID !

            var serializer = new XmlSerializer(typeof(messages));
            messages responseMessages;
            using (TextReader reader = new StringReader(response))
            {
                responseMessages = (messages)serializer.Deserialize(reader);
            }

            // When a client gets a message, it has to call MessageSuccesful Method
            // in order to mark this message as read. It takes Message Id as a
            // parameter and does not returns a value.
            foreach (var OrderItem in responseMessages.order)
            {
                MessageSuccesful(OrderItem.MessageId);
            } // I Get meesage acknowledge

            foreach (var OrderItem in responseMessages.order)
            {
                UpdateOrder(OrderItem.Id, OrderStates.Approved, "");
            }//approved

            foreach (var OrderItem in responseMessages.order)
            {
                UpdateOrder(OrderItem.Id, OrderStates.OnDelivery, "");
            }//OnDelivery

            foreach (var OrderItem in responseMessages.order)
            {
                UpdateOrder(OrderItem.Id, OrderStates.Delivered, "");
            }//Delivered

            return response;
        }

        public void MessageSuccesful(int MessageId)
        {
            MessageSuccessfulRequest messageSuccessfulRequest = new MessageSuccessfulRequest(Auth, MessageId);
            var response = client.MessageSuccessful(messageSuccessfulRequest);
        }

        /*
         * UpdateOrder:
         * When a client processed an order, it have to give feedback to the yemeksepeti.com by using UpdateOrder method. 
         * UpdateOrder method takes OrderId, OrderState and reason as parameters and returns a string. 
         * If process is completed succesfully it returns “OK”. 
         * If there is an error, it returns the error description as string.         
         */
        public void UpdateOrder(decimal OrderId, OrderStates OrderState, string Reason)
        {
            UpdateOrderRequest updateOrderRequest = new UpdateOrderRequest(Auth, OrderId, OrderState, Reason);
            var response = client.UpdateOrder(updateOrderRequest).UpdateOrderResult;
        }

    }

    // V2, V3 Responses are require different models..
    #region Single Message Response
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class order
    {

        private orderProduct productField;

        private int idField;

        private int messageIdField;

        private string restaurantNameField;

        private string restaurantCatalogField;

        private string restaurantCategoryField;

        private string customerNameField;

        private int customerIdField;

        private byte customerTypeField;

        private string paymentNoteField;

        private decimal orderTotalField;

        private string customerPhoneField;

        private string customerPhone2Field;

        private string promoCodeField;

        private string cityField;

        private string regionField;

        private string organizationField;

        private string addressField;

        private string addressDescriptionField;

        private string addressIdField;

        private byte paymentMethodIdField;

        private string deliveryTimeField;

        private decimal changeInTotalField;

        private string currencyField;

        private string orderNoteField;

        /// <remarks/>
        public orderProduct product
        {
            get
            {
                return this.productField;
            }
            set
            {
                this.productField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int MessageId
        {
            get
            {
                return this.messageIdField;
            }
            set
            {
                this.messageIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RestaurantName
        {
            get
            {
                return this.restaurantNameField;
            }
            set
            {
                this.restaurantNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RestaurantCatalog
        {
            get
            {
                return this.restaurantCatalogField;
            }
            set
            {
                this.restaurantCatalogField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RestaurantCategory
        {
            get
            {
                return this.restaurantCategoryField;
            }
            set
            {
                this.restaurantCategoryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CustomerName
        {
            get
            {
                return this.customerNameField;
            }
            set
            {
                this.customerNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int CustomerId
        {
            get
            {
                return this.customerIdField;
            }
            set
            {
                this.customerIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte CustomerType
        {
            get
            {
                return this.customerTypeField;
            }
            set
            {
                this.customerTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string PaymentNote
        {
            get
            {
                return this.paymentNoteField;
            }
            set
            {
                this.paymentNoteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal OrderTotal
        {
            get
            {
                return this.orderTotalField;
            }
            set
            {
                this.orderTotalField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CustomerPhone
        {
            get
            {
                return this.customerPhoneField;
            }
            set
            {
                this.customerPhoneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CustomerPhone2
        {
            get
            {
                return this.customerPhone2Field;
            }
            set
            {
                this.customerPhone2Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string PromoCode
        {
            get
            {
                return this.promoCodeField;
            }
            set
            {
                this.promoCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string City
        {
            get
            {
                return this.cityField;
            }
            set
            {
                this.cityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Region
        {
            get
            {
                return this.regionField;
            }
            set
            {
                this.regionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Organization
        {
            get
            {
                return this.organizationField;
            }
            set
            {
                this.organizationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string AddressDescription
        {
            get
            {
                return this.addressDescriptionField;
            }
            set
            {
                this.addressDescriptionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string AddressId
        {
            get
            {
                return this.addressIdField;
            }
            set
            {
                this.addressIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte PaymentMethodId
        {
            get
            {
                return this.paymentMethodIdField;
            }
            set
            {
                this.paymentMethodIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DeliveryTime
        {
            get
            {
                return this.deliveryTimeField;
            }
            set
            {
                this.deliveryTimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal ChangeInTotal
        {
            get
            {
                return this.changeInTotalField;
            }
            set
            {
                this.changeInTotalField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Currency
        {
            get
            {
                return this.currencyField;
            }
            set
            {
                this.currencyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string OrderNote
        {
            get
            {
                return this.orderNoteField;
            }
            set
            {
                this.orderNoteField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class orderProduct
    {

        private string idField;

        private string nameField;

        private decimal priceField;

        private decimal listPriceField;

        private byte quantityField;

        private string optionsField;

        private string optionIdsField;

        private byte orderIndexField;

        private byte parentIndexField;

        private byte promoParentIndexField;

        private string productOptionIdField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal Price
        {
            get
            {
                return this.priceField;
            }
            set
            {
                this.priceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal ListPrice
        {
            get
            {
                return this.listPriceField;
            }
            set
            {
                this.listPriceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte Quantity
        {
            get
            {
                return this.quantityField;
            }
            set
            {
                this.quantityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Options
        {
            get
            {
                return this.optionsField;
            }
            set
            {
                this.optionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string OptionIds
        {
            get
            {
                return this.optionIdsField;
            }
            set
            {
                this.optionIdsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte OrderIndex
        {
            get
            {
                return this.orderIndexField;
            }
            set
            {
                this.orderIndexField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte ParentIndex
        {
            get
            {
                return this.parentIndexField;
            }
            set
            {
                this.parentIndexField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte PromoParentIndex
        {
            get
            {
                return this.promoParentIndexField;
            }
            set
            {
                this.promoParentIndexField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ProductOptionId
        {
            get
            {
                return this.productOptionIdField;
            }
            set
            {
                this.productOptionIdField = value;
            }
        }
    }
    #endregion

    #region Multiple Messages Response


    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class messages
    {

        private messagesOrder[] orderField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("order")]
        public messagesOrder[] order
        {
            get
            {
                return this.orderField;
            }
            set
            {
                this.orderField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class messagesOrder
    {

        private messagesOrderProduct productField;

        private int idField;

        private int messageIdField;

        private string restaurantNameField;

        private string restaurantCatalogField;

        private string restaurantCategoryField;

        private string customerNameField;

        private int customerIdField;

        private byte customerTypeField;

        private string paymentNoteField;

        private decimal orderTotalField;

        private string customerPhoneField;

        private string customerPhone2Field;

        private string promoCodeField;

        private string cityField;

        private string regionField;

        private string organizationField;

        private string addressField;

        private string addressDescriptionField;

        private string addressIdField;

        private byte paymentMethodIdField;

        private string deliveryTimeField;

        private decimal changeInTotalField;

        private string currencyField;

        private string orderNoteField;

        /// <remarks/>
        public messagesOrderProduct product
        {
            get
            {
                return this.productField;
            }
            set
            {
                this.productField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int MessageId
        {
            get
            {
                return this.messageIdField;
            }
            set
            {
                this.messageIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RestaurantName
        {
            get
            {
                return this.restaurantNameField;
            }
            set
            {
                this.restaurantNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RestaurantCatalog
        {
            get
            {
                return this.restaurantCatalogField;
            }
            set
            {
                this.restaurantCatalogField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RestaurantCategory
        {
            get
            {
                return this.restaurantCategoryField;
            }
            set
            {
                this.restaurantCategoryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CustomerName
        {
            get
            {
                return this.customerNameField;
            }
            set
            {
                this.customerNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int CustomerId
        {
            get
            {
                return this.customerIdField;
            }
            set
            {
                this.customerIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte CustomerType
        {
            get
            {
                return this.customerTypeField;
            }
            set
            {
                this.customerTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string PaymentNote
        {
            get
            {
                return this.paymentNoteField;
            }
            set
            {
                this.paymentNoteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal OrderTotal
        {
            get
            {
                return this.orderTotalField;
            }
            set
            {
                this.orderTotalField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CustomerPhone
        {
            get
            {
                return this.customerPhoneField;
            }
            set
            {
                this.customerPhoneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CustomerPhone2
        {
            get
            {
                return this.customerPhone2Field;
            }
            set
            {
                this.customerPhone2Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string PromoCode
        {
            get
            {
                return this.promoCodeField;
            }
            set
            {
                this.promoCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string City
        {
            get
            {
                return this.cityField;
            }
            set
            {
                this.cityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Region
        {
            get
            {
                return this.regionField;
            }
            set
            {
                this.regionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Organization
        {
            get
            {
                return this.organizationField;
            }
            set
            {
                this.organizationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string AddressDescription
        {
            get
            {
                return this.addressDescriptionField;
            }
            set
            {
                this.addressDescriptionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string AddressId
        {
            get
            {
                return this.addressIdField;
            }
            set
            {
                this.addressIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte PaymentMethodId
        {
            get
            {
                return this.paymentMethodIdField;
            }
            set
            {
                this.paymentMethodIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DeliveryTime
        {
            get
            {
                return this.deliveryTimeField;
            }
            set
            {
                this.deliveryTimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal ChangeInTotal
        {
            get
            {
                return this.changeInTotalField;
            }
            set
            {
                this.changeInTotalField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Currency
        {
            get
            {
                return this.currencyField;
            }
            set
            {
                this.currencyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string OrderNote
        {
            get
            {
                return this.orderNoteField;
            }
            set
            {
                this.orderNoteField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class messagesOrderProduct
    {

        private messagesOrderProductOption[] optionField;

        private string idField;

        private string nameField;

        private decimal priceField;

        private decimal listPriceField;

        private byte quantityField;

        private string optionsField;

        private string optionIdsField;

        private byte orderIndexField;

        private byte parentIndexField;

        private byte promoParentIndexField;

        private string productOptionIdField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("option")]
        public messagesOrderProductOption[] option
        {
            get
            {
                return this.optionField;
            }
            set
            {
                this.optionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal Price
        {
            get
            {
                return this.priceField;
            }
            set
            {
                this.priceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal ListPrice
        {
            get
            {
                return this.listPriceField;
            }
            set
            {
                this.listPriceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte Quantity
        {
            get
            {
                return this.quantityField;
            }
            set
            {
                this.quantityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Options
        {
            get
            {
                return this.optionsField;
            }
            set
            {
                this.optionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string OptionIds
        {
            get
            {
                return this.optionIdsField;
            }
            set
            {
                this.optionIdsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte OrderIndex
        {
            get
            {
                return this.orderIndexField;
            }
            set
            {
                this.orderIndexField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte ParentIndex
        {
            get
            {
                return this.parentIndexField;
            }
            set
            {
                this.parentIndexField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte PromoParentIndex
        {
            get
            {
                return this.promoParentIndexField;
            }
            set
            {
                this.promoParentIndexField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ProductOptionId
        {
            get
            {
                return this.productOptionIdField;
            }
            set
            {
                this.productOptionIdField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class messagesOrderProductOption
    {

        private string idField;

        private string nameField;

        private byte productOrderIdField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte ProductOrderId
        {
            get
            {
                return this.productOrderIdField;
            }
            set
            {
                this.productOrderIdField = value;
            }
        }
    }

    #endregion


}

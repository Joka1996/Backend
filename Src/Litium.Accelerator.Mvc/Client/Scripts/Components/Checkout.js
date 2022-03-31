import React, {
    Fragment,
    useCallback,
    useEffect,
    useRef,
    useMemo,
} from 'react';

import Cart from './Cart';
import CheckoutPrivateCustomerInfo from './Checkout.PrivateCustomerInfo';
import CheckoutBusinessCustomerInfo from './Checkout.BusinessCustomerInfo';
import CheckoutDeliveryMethods from './Checkout.DeliveryMethods';
import CheckoutPaymentMethods from './Checkout.PaymentMethods';
import CheckoutOrderNote from './Checkout.OrderNote';
import CheckoutOrderInfo from './Checkout.OrderInfo';
import PaymentWidget from './Payments/PaymentWidget';
import CheckoutDiscountCodes from './Checkout.DiscountCodes';

import constants from '../constants';

import {
    acceptTermsOfCondition,
    setBusinessCustomer,
    setDelivery,
    setPayment,
    reloadPayment,
    submit,
    setCustomerDetails,
    setAlternativeAddress,
    submitError,
    submitRequest,
    submitDone,
} from '../Actions/Checkout.action';

import { translate } from '../Services/translation';
import { string, object, boolean, mixed } from 'yup';
import { useSelector, useDispatch } from 'react-redux';

const privateCustomerAdditionalDetailsSchema = object().shape({
    acceptTermsOfCondition: boolean()
        .required(translate(`validation.checkrequired`))
        .oneOf([true], translate(`validation.checkrequired`)),
    selectedDeliveryMethod: string().required(translate(`validation.required`)),
    selectedPaymentMethod: string().required(translate(`validation.required`)),
});

const privateCustomerAddressSchema = object().shape({
    email: string()
        .required(translate(`validation.required`))
        .email(translate(`validation.email`)),
    phoneNumber: string().required(translate(`validation.required`)),
    country: mixed()
        .required(translate(`validation.required`))
        .notOneOf([''], translate('validation.required')),
    city: string().required(translate(`validation.required`)),
    zipCode: string().required(translate(`validation.required`)),
    address: string().required(translate(`validation.required`)),
    lastName: string().required(translate(`validation.required`)),
    firstName: string().required(translate(`validation.required`)),
});

const privateCustomerAlternativeAddressSchema = object().shape({
    phoneNumber: string().required(translate(`validation.required`)),
    country: mixed()
        .required(translate(`validation.required`))
        .notOneOf([''], translate('validation.required')),
    city: string().required(translate(`validation.required`)),
    zipCode: string().required(translate(`validation.required`)),
    address: string().required(translate(`validation.required`)),
    lastName: string().required(translate(`validation.required`)),
    firstName: string().required(translate(`validation.required`)),
});

const businessCustomerDetailsSchema = object().shape({
    acceptTermsOfCondition: boolean()
        .required(translate(`validation.checkrequired`))
        .oneOf([true], translate(`validation.checkrequired`)),
    selectedDeliveryMethod: string().required(translate(`validation.required`)),
    selectedPaymentMethod: string().required(translate(`validation.required`)),
    email: string()
        .required(translate(`validation.required`))
        .email(translate(`validation.email`)),
    phoneNumber: string().required(translate(`validation.required`)),
    lastName: string().required(translate(`validation.required`)),
    firstName: string().required(translate(`validation.required`)),
    selectedCompanyAddressId: string().required(
        translate(`validation.required`)
    ),
});

const Checkout = () => {
    const dispatch = useDispatch();
    const cart = useSelector((state) => state.cart);
    const checkout = useSelector((state) => state.checkout);

    const onSubmit = useCallback(() => dispatch(submit()), [dispatch]);
    const onSubmitError = useCallback(
        (error) => {
            dispatch(submitError(error));
            dispatch(submitDone(null));
        },
        [dispatch]
    );
    const onCustomerDetailsChangeCb = useCallback(
        (stateKey, propName, value) => {
            switch (stateKey) {
                case 'customerDetails':
                    dispatch(setCustomerDetails(propName, value));
                    break;
                case 'alternativeAddress':
                    dispatch(setAlternativeAddress(propName, value));
                    break;
            }
        },
        [dispatch]
    );

    const placeOrder = useCallback(() => {
        const { payload } = checkout,
            {
                isBusinessCustomer,
                selectedCompanyAddressId,
                acceptTermsOfCondition,
                selectedPaymentMethod,
                selectedDeliveryMethod,
            } = payload;
        const notCustomerDetailFields = [
            'selectedCompanyAddressId',
            'selectedPaymentMethod',
            'selectedDeliveryMethod',
            'acceptTermsOfCondition',
        ];
        const onError = (error, addressPath = 'customerDetails') => {
            error.path =
                notCustomerDetailFields.indexOf(error.path) >= 0
                    ? error.path
                    : `${addressPath}-${error.path}`;
            onSubmitError(error);
        };
        dispatch(submitRequest());
        if (isBusinessCustomer) {
            businessCustomerDetailsSchema
                .validate({
                    ...payload.customerDetails,
                    selectedCompanyAddressId,
                    selectedPaymentMethod,
                    selectedDeliveryMethod,
                    acceptTermsOfCondition,
                })
                .then(() => {
                    onSubmit();
                })
                .catch(onError);
        } else {
            const checkAltAddress =
                payload.alternativeAddress.showAlternativeAddress &&
                (payload.alternativeAddress.firstName ||
                    payload.alternativeAddress.lastName ||
                    payload.alternativeAddress.address ||
                    payload.alternativeAddress.zipCode ||
                    payload.alternativeAddress.city ||
                    payload.alternativeAddress.phoneNumber);

            privateCustomerAddressSchema
                .validate({
                    ...payload.customerDetails,
                })
                .then(() => {
                    payload.showAlternativeAddress =
                        payload.alternativeAddress.showAlternativeAddress;
                    if (checkAltAddress) {
                        privateCustomerAlternativeAddressSchema
                            .validate({
                                ...payload.alternativeAddress,
                            })
                            .then(() => {
                                privateCustomerAdditionalDetailsSchema
                                    .validate({
                                        selectedPaymentMethod,
                                        selectedDeliveryMethod,
                                        acceptTermsOfCondition,
                                    })
                                    .then(() => {
                                        onSubmit();
                                    })
                                    .catch(onError);
                            })
                            .catch((error) => {
                                onError(error, 'alternativeAddress');
                            });
                    } else {
                        privateCustomerAdditionalDetailsSchema
                            .validate({
                                selectedPaymentMethod,
                                selectedDeliveryMethod,
                                acceptTermsOfCondition,
                            })
                            .then(() => {
                                onSubmit();
                            })
                            .catch(onError);
                    }
                })
                .catch(onError);
        }
    }, [checkout, onSubmit, onSubmitError, dispatch]);

    const firstRender = useRef(true);

    useEffect(() => {
        if (!firstRender.current) {
            return;
        }
        firstRender.current = false;

        if (!checkout) {
            return;
        }

        const {
            selectedPaymentMethod,
            selectedDeliveryMethod,
            customerDetails,
            alternativeAddress,
        } = checkout.payload;
        // set selected value for payment method on load.
        selectedPaymentMethod && dispatch(setPayment(selectedPaymentMethod));
        // set selected value for delivery method on load.
        selectedDeliveryMethod && dispatch(setDelivery(selectedDeliveryMethod));

        // fill default select value to the state
        (!customerDetails || !customerDetails.country) &&
            constants.countries &&
            constants.countries[0] &&
            onCustomerDetailsChangeCb(
                'customerDetails',
                'country',
                constants.countries[0].value
            );
        (!alternativeAddress || !alternativeAddress.country) &&
            constants.countries &&
            constants.countries[0] &&
            onCustomerDetailsChangeCb(
                'alternativeAddress',
                'country',
                constants.countries[0].value
            );
    }, [checkout, dispatch, onCustomerDetailsChangeCb]);

    useEffect(() => {
        if (checkout.result && checkout.result.redirectUrl) {
            window.location = checkout.result.redirectUrl;
            return;
        }

        if (!checkout.isSubmitting || !checkout.errors) {
            return;
        }

        const errorKeys = Object.keys(checkout.errors);
        if (!errorKeys || errorKeys.length < 1) {
            return;
        }

        const errorNode = document.querySelector(
            `[data-error-for="${errorKeys[0]}"]`
        );
        if (!errorNode) {
            return;
        }

        const inputNode = errorNode.parentElement.querySelector('input');
        if (inputNode) {
            setTimeout(() => inputNode.focus(), 1000);
            inputNode.scrollIntoView({ behavior: 'smooth' });
        } else {
            errorNode.scrollIntoView({ behavior: 'smooth' });
        }
    }, [checkout]);

    const cartSection = useMemo(() => {
        const { errors = {} } = checkout;
        return (
            <Fragment>
                <div className="row">
                    <div className="small-12">
                        <h2 className="checkout__title">
                            {translate('checkout.title')}
                        </h2>
                    </div>
                </div>
                <div className="row">
                    <h3 className="checkout__section-title">
                        {translate('checkout.cart.title')}
                    </h3>
                </div>
                <div className="row">
                    {errors && errors['cart'] && (
                        <p className="checkout__validator--error">
                            {errors['cart'][0]}
                        </p>
                    )}
                </div>
                <Cart />
            </Fragment>
        );
    }, [checkout]);

    const customerDetailsSection = useMemo(() => {
        const {
            authenticated,
            isBusinessCustomer,
            checkoutMode,
            loginUrl,
        } = checkout.payload;

        const privateCustomerInfoComponent = (
            <CheckoutPrivateCustomerInfo onChange={onCustomerDetailsChangeCb} />
        );
        const businessCustomerInfoComponent = (
            <CheckoutBusinessCustomerInfo
                onChange={onCustomerDetailsChangeCb}
            />
        );
        if (!authenticated) {
            return (
                <Fragment>
                    <div className="row">
                        <h3 className="checkout__section-title">
                            {translate('checkout.customerinfo.title')}
                        </h3>
                        <Fragment>
                            <label className="checkout__text--in-line">
                                {translate(
                                    'checkout.customerinfo.existingcustomer'
                                )}
                            </label>
                            &nbsp;
                            <a href={loginUrl} className="checkout__link">
                                {translate(
                                    'checkout.customerinfo.clicktologin'
                                )}
                            </a>
                            &nbsp;
                            {!isBusinessCustomer &&
                                checkoutMode ===
                                    constants.checkoutMode.both && (
                                    <a
                                        onClick={() =>
                                            dispatch(setBusinessCustomer(true))
                                        }
                                        className="checkout__link"
                                    >
                                        {translate(
                                            'checkout.customerinfo.businesscustomer'
                                        )}
                                    </a>
                                )}
                        </Fragment>
                        {isBusinessCustomer &&
                            checkoutMode === constants.checkoutMode.both && (
                                <a
                                    onClick={() =>
                                        dispatch(setBusinessCustomer(false))
                                    }
                                    className="checkout__link"
                                >
                                    {translate(
                                        'checkout.customerinfo.privatecustomer'
                                    )}
                                </a>
                            )}
                    </div>
                    {!isBusinessCustomer &&
                        checkoutMode !==
                            constants.checkoutMode.companyCustomers &&
                        privateCustomerInfoComponent}
                    {(isBusinessCustomer ||
                        checkoutMode ===
                            constants.checkoutMode.companyCustomers) &&
                        businessCustomerInfoComponent}
                </Fragment>
            );
        }
        if (isBusinessCustomer) {
            return (
                <Fragment>
                    <div className="row">
                        <h3 className="checkout__section-title">
                            {translate('checkout.customerinfo.title')}
                        </h3>
                    </div>
                    {authenticated && businessCustomerInfoComponent}
                </Fragment>
            );
        }

        return (
            <Fragment>
                <div className="row">
                    <h3 className="checkout__section-title">
                        {translate('checkout.customerinfo.title')}
                    </h3>
                </div>
                {privateCustomerInfoComponent}
            </Fragment>
        );
    }, [checkout, dispatch, onCustomerDetailsChangeCb]);

    if (!cart || !cart.orderRows || cart.orderRows.length < 1) {
        return (
            <div className="row">
                <div className="small-12">
                    <h2 className="checkout__title">
                        {translate(`checkout.cart.empty`)}
                    </h2>
                </div>
            </div>
        );
    }

    const { payload, errors = {} } = checkout,
        {
            paymentWidget,
            paymentMethods,
            authenticated,
            isBusinessCustomer,
            checkoutMode,
        } = payload;
    const responseString = paymentWidget ? paymentWidget.responseString : null;
    const updateKey = paymentWidget ? paymentWidget._force_update : null;

    return (
        <Fragment>
            {(!paymentWidget || paymentWidget.isChangeable) && cartSection}
            {(!paymentWidget || paymentWidget.displayCustomerDetails) &&
                customerDetailsSection}

            <div className="row">
                <h3 className="checkout__section-title">
                    {translate('checkout.delivery.title')}
                </h3>
            </div>
            {(!paymentWidget || paymentWidget.displayDeliveryMethods) && (
                <CheckoutDeliveryMethods />
            )}
            {errors['selectedDeliveryMethod'] && (
                <span className="form__validator--error form__validator--top-narrow">
                    {errors['selectedDeliveryMethod'][0]}
                </span>
            )}

            <div className="row">
                <h3 className="checkout__section-title">
                    {translate('checkout.payment.title')}
                </h3>
            </div>
            {paymentMethods &&
                paymentMethods.length > 0 &&
                (!paymentWidget || paymentWidget.isChangeable) && (
                    <CheckoutPaymentMethods />
                )}
            {errors['selectedPaymentMethod'] && (
                <span className="form__validator--error form__validator--top-narrow">
                    {errors['selectedPaymentMethod'][0]}
                </span>
            )}
            <div className="row">
                <h3 className="checkout__section-title">
                    {translate('checkout.discountcode')}
                </h3>
            </div>
            <CheckoutDiscountCodes />

            {paymentWidget && !paymentWidget.hidePaymentWidget && (
                <PaymentWidget
                    key={updateKey}
                    responseString={responseString}
                />
            )}

            {!paymentWidget && (
                <Fragment>
                    <div className="row">
                        <h3 className="checkout__section-title">
                            {translate('checkout.order.title')}
                        </h3>
                    </div>

                    <section className="row checkout-info__container checkout-info__summary">
                        <CheckoutOrderNote />
                        <CheckoutOrderInfo />
                    </section>

                    <div className="row">
                        <input
                            className="checkout-info__checkbox-input"
                            type="checkbox"
                            id="acceptTermsOfCondition"
                            checked={payload.acceptTermsOfCondition}
                            onChange={(event) =>
                                dispatch(
                                    acceptTermsOfCondition(event.target.checked)
                                )
                            }
                        />
                        <label
                            className="checkout-info__checkbox-label"
                            htmlFor="acceptTermsOfCondition"
                        >
                            {translate('checkout.terms.acceptTermsOfCondition')}{' '}
                            <a
                                className="checkout__link"
                                href={payload.termsUrl}
                                target="_blank"
                                rel="noreferrer"
                            >
                                {translate('checkout.terms.link')}
                            </a>
                        </label>
                        {errors['acceptTermsOfCondition'] && (
                            <span
                                className="form__validator--error form__validator--top-narrow"
                                data-error-for="acceptTermsOfCondition"
                            >
                                {errors['acceptTermsOfCondition'][0]}
                            </span>
                        )}
                    </div>

                    <div className="row">
                        {!authenticated &&
                        (isBusinessCustomer ||
                            checkoutMode ===
                                constants.checkoutMode.companyCustomers) ? (
                            <button
                                className="checkout__submit-button"
                                onClick={() =>
                                    (location.href = payload.loginUrl)
                                }
                            >
                                {translate('checkout.login.to.placeorder')}
                            </button>
                        ) : (
                            <button
                                type="submit"
                                className="checkout__submit-button"
                                disabled={checkout.isSubmitting}
                                onClick={placeOrder}
                            >
                                {translate('checkout.placeorder')}
                            </button>
                        )}
                    </div>
                </Fragment>
            )}

            <div className="row">
                {errors && errors['general'] && (
                    <p className="checkout__validator--error">
                        {errors['general'][0]}
                    </p>
                )}
                {errors && errors['payment'] && (
                    <p className="checkout__validator--error">
                        {errors['payment'][0]}
                    </p>
                )}
            </div>
        </Fragment>
    );
};

export default Checkout;
